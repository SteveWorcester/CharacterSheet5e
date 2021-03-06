﻿using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Roll20Roller.Importer.Base;
using Roll20Roller.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roll20Roller.Importer.Maps
{
    public class SpellsObjects : ScraperBase
    {
        // navigation buttons
        protected IWebElement BtnSpellsTabButtonPreClick => _Driver.WaitForElement(By.CssSelector(".ct-primary-box__tab--spells"), 45000);
        protected IWebElement BtnSpellsTabButtonPostClick => _Driver.WaitForElement(By.CssSelector("div.ct-primary-box__tab--spells:nth-child(2)"), 45000);

        // basic content - main spells window
        private IWebElement _basicSpellsParent => _Driver.WaitForElement(By.CssSelector(".ddbc-tab-options__content"));
        public IReadOnlyCollection<IWebElement> AllSpellNames => _basicSpellsParent.FindElements(By.XPath("./div/div[2]/div[1]/div//span[@class=\" ddbc-spell-name\"]"));
        protected IWebElement SpellLineParentByName(string spellName) => AllSpellNames
            .FirstOrDefault(n => n.Text.Contains(spellName))
            .FindElement(By.XPath("./parent::div/parent::div/parent::div"));
        protected IWebElement SpellSpecificSaveDcStat(string spellName) => SpellLineParentByName(spellName).FindElement(By.XPath("./div[5]/div/span[1]"));
        protected IWebElement SpellSpecificSaveDcNumber(string spellName) => SpellLineParentByName(spellName).FindElement(By.XPath("./div[5]/div/span[2]"));
        protected List<IWebElement> SpellLevelGroups => _basicSpellsParent.FindElements(By.XPath("./div")).ToList();
        protected List<IWebElement> SpellNamesByLevel(int level) => SpellLevelGroups[level].FindElements(By.XPath("./div[2]/div[1]/div[2]/div//span[@class=\" ddbc-spell-name\"]")).ToList();
        protected IWebElement _spellBonusParent => _Driver.WaitForElement(By.CssSelector(".ct-spells-level-casting__info"));
        protected IWebElement SpellModifier => _spellBonusParent.FindElement(By.XPath("./div[1]/div[1]/span/span/span[2]"));
        protected IWebElement SpellModifierPlusMinus => _spellBonusParent.FindElement(By.XPath("./div[1]/div[1]/span/span/span[1]"));
        protected IWebElement SpellAttack => _spellBonusParent.FindElement(By.XPath("./div[2]/div[1]/span/span/span[2]"));
        protected IWebElement SpellAttackPlusMinus => _spellBonusParent.FindElement(By.XPath("./div[2]/div[1]/span/span/span[1]"));
        protected IWebElement SaveDc => _spellBonusParent.FindElement(By.XPath("./div[3]/div[1]/span"));

        // detailed content - side pane
        private IWebElement _detailedSpellsParent => _Driver.WaitForElement(By.CssSelector(".ct-spell-detail"));
        protected IWebElement SelectedSpellClass => _detailedSpellsParent.FindElement(By.XPath("./parent::div/div[1]/div[1]"));
        protected IWebElement SelectedSpellName => _detailedSpellsParent.FindElement(By.XPath("./parent::div/div[1]/div[2]/div[2]/span"));
        protected IWebElement SelectedSpellSchool => _detailedSpellsParent.FindElement(By.XPath("./div[1]/span[2]"));
        protected IWebElement SelectedSpellLevel => _detailedSpellsParent.FindElement(By.XPath("./div[1]/span[1]"));
        protected IWebElement SelectedSpellCastingTime => _detailedSpellsParent.FindElement(By.CssSelector("div.ddbc-property-list__property:nth-child(1) > div:nth-child(2)"));
        protected IWebElement SelectedSpellRangeArea => IsTouchSpell()
            ? _touch
            : _ranged;
        protected IReadOnlyCollection<IWebElement> SelectedSpellComponentTypes => _detailedSpellsParent.FindElements(By.CssSelector(".ct-spell-detail__components > span"));
        protected IWebElement SelectedSpellComponentMaterials => _detailedSpellsParent.FindElement(By.CssSelector(".ct-spell-detail__components-description"));
        protected IWebElement SelectedSpellDuration => _detailedSpellsParent.FindElement(By.CssSelector("div.ddbc-property-list__property:nth-child(4) > div:nth-child(2)"));
        protected IWebElement SelectedSpellDetail => _detailedSpellsParent.FindElement(By.CssSelector(".ddbc-html-content > p:nth-child(1)"));
        protected IWebElement SelectedSpellDetailHigherLevels => _detailedSpellsParent.FindElement(By.XPath("./div[5]/p[2]"));

        internal bool HasMaterialComponents()
        {
            try
            {
                var checkText = SelectedSpellComponentMaterials.Text;
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        internal bool HasHigherLevelsDescription()
        {
            try
            {
                var checkText = SelectedSpellDetailHigherLevels.Text;
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        private bool IsTouchSpell()
        {
            try
            {
                var element = _ranged.Text;
                return false;
            }
            catch (NoSuchElementException)
            {
                return true;
            }
        }
        private IWebElement _ranged => _detailedSpellsParent.FindElement(By.CssSelector("div.ddbc-property-list__property:nth-child(2) > div:nth-child(2) > span:nth-child(1) > span:nth-child(1)"));
        private IWebElement _touch => _detailedSpellsParent.FindElement(By.CssSelector("div.ddbc-property-list__property:nth-child(2) > div:nth-child(2)"));
        
        public bool HasAvailableSpells()
        {
            try
            {
                var element = BtnSpellsTabButtonPreClick.Text;
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public bool IsSpellsTabSelected()
        {
            try
            {
                var element = BtnSpellsTabButtonPostClick.Text;
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public bool HasSpellModifier()
        {
            if (IsSpellsTabSelected())
            {
                try
                {
                    System.Threading.Thread.Sleep(50);
                    // SpellBonusParent with no wait.
                    var element = _Driver.FindElement(By.CssSelector("div.ct-spells-level-casting__info-group:nth-child(1) > div:nth-child(1) > span:nth-child(1) > span:nth-child(1) > span:nth-child(2)")).Text;
                    return true;
                }
                catch (NoSuchElementException)
                {
                    return false;
                }
            }
            return false;
        }

        public bool HasSpellSpecificDc(Spell spell)
        {
            if (IsSpellsTabSelected())
            {
                try
                {
                    // spell-specific hit/dc no wait.
                    var element = _Driver.FindElement(By.CssSelector(".ddbc-tab-options__content"))
                        .FindElements(By.XPath("./div/div[2]/div[1]/div//span[@class=\" ddbc-spell-name\"]"))
                        .FirstOrDefault(n => n.Text.Contains(spell.Name))
                        .FindElement(By.XPath("./parent::div/parent::div/parent::div/div[5]/div/span[2]")).Text;
                    return true;
                }
                catch (NoSuchElementException)
                {
                    return false;
                }
            }
            return false;
        }
    }
}
