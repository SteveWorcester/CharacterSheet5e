﻿using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Roll20Roller.Importer.Base
{
    public static class Wait
    {
        public static IWebElement WaitForElement(this IWebDriver driver, By by, int timeoutInMs = 45000) // 45 seconds
        {
            return new WebDriverWait(driver, TimeSpan.FromMilliseconds(timeoutInMs)).Until((drv) => {
                try
                {
                    var ele = drv.FindElement(by);
                    return ele.Displayed ? ele : null;
                }
                catch (StaleElementReferenceException)
                {
                    return null;
                }
                catch (NotFoundException)
                {
                    return null;
                }
            });
        }
    }
}
