﻿using OpenQA.Selenium;
using Roll20Roller.Enums;
using Roll20Roller.Importer.Base;
using Roll20Roller.Importer.Maps;
using Roll20Roller.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Roll20Roller.Importer.Actions
{
    public class SpellsFromDdbActions : SpellsObjects
    {
        private List<(CharacterClass charClass, int charLevel)> _classNamesAndLevels;
        private MainPageActions mainPage;
        public SpellsFromDdbActions(long charId)
        {
            SetCharacterAndPullData(charId);
            mainPage = new MainPageActions(charId);
            _classNamesAndLevels = mainPage.GetClassNamesAndLevels();
        }

        public List<string> GetSpellsListForDropdown()
        {
            var spellsList = new List<string>();
            for (int i = 0; i < SpellLevelGroups.Count; i++)
            {
                if (i == 0)
                {
                    spellsList.Add("====Cantrip====");
                }
                if (i > 0)
                {
                    spellsList.Add(string.Empty);
                    spellsList.Add($"===Level {i}===");
                }

                foreach (var nameElement in SpellNamesByLevel(i))
                {
                    if (!spellsList.Contains(nameElement.Text))
                    {
                        spellsList.Add(nameElement.GetAttribute("class"));
                    }                    
                }
            }

            return spellsList;
        }

        internal void SetSpellsList(ComboBox ddlDdbSpells)
        {
            var spellsBindingSource = new BindingSource();
            spellsBindingSource.DataSource = GetSpellsListForDropdown();

            ddlDdbSpells.ValueMember = "Name";
            ddlDdbSpells.DisplayMember = "Name";
            ddlDdbSpells.DataSource = spellsBindingSource.DataSource;
        }

        internal void Init(ComboBox ddlDdbSpells)
        {
            BtnSpellsTabButtonPreClick.Click();
            SetSpellsList(ddlDdbSpells);
            ddlDdbSpells.SelectedIndex = 1;
        }

        public Spell GetSpellFromDdb(string spellName)
        {
            if (spellName.Equals(string.Empty) || spellName.Contains("=-"))
            {
                return new Spell();
            }

            SpellLineParentByName(spellName).Click();

            if (!Enum.TryParse<CharacterClass>(SelectedSpellClass.Text, out var className))
            {
                throw new FormatException($"The class {SelectedSpellClass.Text} cannot be parsed in spell description.");
            }

            var spellSchool = SelectedSpellSchool.Text;
            if (!int.TryParse(SelectedSpellLevel.Text.First().ToString(), out var levelInt))
            {
                // Cantrips school/level are backwards. ex: `1st level Necromancy` -vs- `Necromancy Cantrip`
                if (SelectedSpellSchool.Text.Contains("Cantrip"))
                {
                    levelInt = 0;
                    spellSchool = SelectedSpellLevel.Text;
                }
                else
                {
                    throw new FormatException($"unable to parse level {SelectedSpellLevel.Text} into character level for spells.");
                }
            }

            return new Spell()
            {
                CastingTime = SelectedSpellCastingTime.Text,
                Class = className,
                ComponentTypes = SelectedSpellComponentTypes.Select(t => t.Text).ToList(),
                ComponentMaterials = HasMaterialComponents() ? SelectedSpellComponentMaterials.Text:"None",
                Description = SelectedSpellDetail.Text,
                DescriptionHigherLevels = HasHigherLevelsDescription() ? SelectedSpellDetailHigherLevels.Text : "None",
                Duration = SelectedSpellDuration.Text,
                IsConcentration = SelectedSpellDuration.Text.Contains("Concentration"),
                Level = levelInt,
                Name = SelectedSpellName.Text,
                Range = SelectedSpellRangeArea.Text,
                School = spellSchool
            };
        }
    }
}
