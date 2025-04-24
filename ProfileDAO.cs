using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
namespace GSM_BYTUTJ2025.DAO
{
    public class ProfileDAO
    {
        private static ProfileDAO instance;

        public static ProfileDAO Instance
        {
            get { if (instance == null) instance = new ProfileDAO(); return ProfileDAO.instance; }
            private set { ProfileDAO.instance = value; }
        }
        public string ConvertToCookie(string cookie)
        {
            try
            {
                string cUser = "c_user=";
                string xs = "xs=";
                string[] cookieArr = cookie.Split(';');

                foreach (string item in cookieArr)
                {
                    string trimmedItem = item.Trim();
                    if (trimmedItem.Contains("c_user="))
                    {
                        cUser += trimmedItem.Split(new[] { "c_user=" }, StringSplitOptions.None)[1];
                    }
                    if (trimmedItem.Contains("xs="))
                    {
                        string xsValue = trimmedItem.Split(new[] { "xs=" }, StringSplitOptions.None)[1];
                        xsValue = xsValue.Contains("|") ? xsValue.Split('|')[0] : xsValue;
                        if (!xsValue.EndsWith(";")) xsValue += ";";
                        xs += xsValue;
                    }
                }

                string conv = cUser + " " + xs;
                if (cUser == "c_user=") return null;
                return conv;
            }
            catch(Exception ex)
            {            
                Libary.Instance.CreateLog("Error Convert Cookie",ex);
                return null;
            }
        }
        public void LoginFacebookByCookie(IWebDriver driver, string cookie)
        {
            try
            {
                string convertedCookie = ConvertToCookie(cookie);
                Console.WriteLine(convertedCookie);

                if (convertedCookie != null)
                {
                    string script = $@"
                javascript:void(function() {{
                    function setCookie(t) {{
                        var list = t.split(""; "");
                        for (var i = list.length - 1; i >= 0; i--) {{
                            var cname = list[i].split(""="")[0];
                            var cvalue = list[i].split(""="")[1];
                            var d = new Date();
                            d.setTime(d.getTime() + (7*24*60*60*1000));
                            var expires = "";domain=.facebook.com;expires="" + d.toUTCString();
                            document.cookie = cname + ""="" + cvalue + ""; "" + expires;
                        }}
                    }}
                    function hex2a(hex) {{
                        var str = """";
                        for (var i = 0; i < hex.length; i += 2) {{
                            var v = parseInt(hex.substr(i, 2), 16);
                            if (v) str += String.fromCharCode(v);
                        }}
                        return str;
                    }}
                    setCookie(""{convertedCookie}"");
                    location.href = ""https://mbasic.facebook.com"";
                }})();";

                    ((IJavaScriptExecutor)driver).ExecuteScript(script);
                    Thread.Sleep(5000);
                }
            }
            catch(Exception ex)
            {
                Libary.Instance.CreateLog("Đăng nhập không thành công", ex);
            }
        }
        public bool CheckProfile(ChromeDriver driver)
        {
            try
            {
                driver.Navigate().GoToUrl("https://www.facebook.com/");

                // Chờ tối đa 5 giây để tải xong DOM
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                wait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));

                string pageSource = driver.PageSource;

                if (pageSource.Contains("Đăng nhập") || pageSource.Contains("Log in"))
                {
                    Libary.Instance.CreateLog("⚠ Profile chưa đăng nhập! Kiểm tra lại đường dẫn profile.");
                    return false;
                }

                Libary.Instance.CreateLog("✅ Đã đăng nhập Facebook thành công.");
                return true;
            }
            catch (Exception ex)
            {
                Libary.Instance.CreateLog($"❌ Lỗi khi kiểm tra profile: {ex.Message}");
                return false;
            }
        }
    }
}
