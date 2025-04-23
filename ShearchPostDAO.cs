using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace CrawFB.DAO
{
    public class ShearchPostDAO
    {
        private static ShearchPostDAO instance;
        public static ShearchPostDAO Instance
        {
            get { if (instance == null) instance = new ShearchPostDAO(); return ShearchPostDAO.instance; }
            private set { ShearchPostDAO.instance = value; }
        }
        public string ExtractFbShortLink(string url)
        {
            // Kiểm tra nếu có ID
            var matchId = Regex.Match(url, @"id=(\d+)");
            if (matchId.Success)
            {
                return $"https://fb.com/{matchId.Groups[1].Value}";
            }

            // Nếu không có ID, lấy tên rút gọn
            var matchShortName = Regex.Match(url, @"facebook\.com/([^/?]+)");
            if (matchShortName.Success)
            {
                return $"https://fb.com/{matchShortName.Groups[1].Value}";
            }

            return "Invalid URL";
        }
        public string GetFullPostContent(IWebElement element)
        {
            string fullContent = "";
            try
            {
                // Tìm tất cả các thẻ span chứa nội dung bài viết
                var contentElements = element.FindElements(By.CssSelector("span[class = 'x193iq5w xeuugli x13faqbe x1vvkbs x1xmvt09 x1lliihq x1s928wv xhkezso x1gmr53x x1cpjm7i x1fgarty x1943h6x xudqn12 x3x7a5m x6prxxf xvq8zen xo1l8bm xzsf02u x1yc453h']"));

                foreach (var content in contentElements)
                {
                    if (content.Text.Contains("Xem thêm"))
                    {
                        try
                        {
                            // Tìm nút "Xem thêm" bên trong phần tử và click
                            var seeMoreButton = content.FindElement(By.CssSelector("div[role='button']"));
                            Thread.Sleep(1000);
                            seeMoreButton.Click();
                            Thread.Sleep(2000); // Chờ nội dung mở rộng
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Lỗi khi click 'Xem thêm': " + ex.Message);
                        }
                    }
                    fullContent += content.Text.Trim() + "\n";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi lấy nội dung bài viết: " + ex.Message);
            }

            return fullContent.Trim();
        }
        public (string trangthai, string userName, string userLink) GetPostInfo(IWebElement postElement)
        {
            string trangthai = "Không xác định";
            string userName = string.Empty;
            string userLink = string.Empty;
            try {
                // Tìm user thông thường
                var userElement = postElement.FindElements(By.CssSelector("span[class='xjp7ctv'] > a"));
                if (userElement.Count > 0)
                {
                    userName = userElement[0].Text.Trim();
                    userLink = ShearchPostDAO.Instance.ExtractFbShortLink(userElement[0].GetAttribute("href"));
                    trangthai = "Bài cá nhân, Page tự đăng";
                }
                else
                {
                    // Tìm user ở bài đăng đặc biệt
                    var specialpost = postElement.FindElements(By.CssSelector("span[class='xjp7ctv']>span>span>a"));
                    if (specialpost.Count > 0)
                    {
                        userName = specialpost[0].Text.Trim();
                        userLink = ShearchPostDAO.Instance.ExtractFbShortLink(specialpost[0].GetAttribute("href"));
                        trangthai = "Bài đăng đặc biệt";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Lỗi nam trong GetPostInfo: " + ex.Message);
            }


            return (trangthai, userName, userLink);
        }
        public string ShortenFacebookPostLink(string originalLink)
        {
            if (string.IsNullOrEmpty(originalLink))
            {
                Console.WriteLine("⚠️ Link rỗng hoặc null.");
                return originalLink;
            }

            try
            {
                // Tìm chỉ số xuất hiện đầu tiên giữa ?__ và &__
                int indexQuestion = originalLink.IndexOf("?__");
                int indexAmp = originalLink.IndexOf("&__");

                int cutIndex = -1;

                if (indexQuestion != -1 && indexAmp != -1)
                {
                    cutIndex = Math.Min(indexQuestion, indexAmp);
                }
                else if (indexQuestion != -1)
                {
                    cutIndex = indexQuestion;
                }
                else if (indexAmp != -1)
                {
                    cutIndex = indexAmp;
                }

                // Cắt chuỗi tại vị trí sớm nhất nếu có
                if (cutIndex != -1)
                {
                    originalLink = originalLink.Substring(0, cutIndex);
                }

                // Thay thế domain Facebook về dạng rút gọn
                originalLink = originalLink.Replace("https://www.facebook.com/", "https://fb.com/");
                originalLink = originalLink.Replace("https://web.facebook.com/", "https://fb.com/");

                return originalLink;
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Lỗi trong ShortenFacebookPostLink: " + ex.Message);
                return originalLink;
            }
        }
        public class PostTypeResult
        {
            public string PostType { get; set; }              // "original", "share", "unknown"
            public string ShareTime { get; set; }             // Thời gian share
            public string OriginalTime { get; set; }          // Thời gian gốc
            public string LinkBaiViet { get; set; }           // Link hiển thị (bài gốc hoặc bài share)
            public string SharePostLink { get; set; }         // Link bài share
            public string OriginalPostLink { get; set; }      // Link bài gốc
        }
        public PostTypeResult PostTypeDetector(List<string> timeList, List<string> linkList)
        {
            var result = new PostTypeResult
            {
              
                ShareTime = "N/A",
                OriginalTime = "N/A",
                LinkBaiViet = "N/A",
                SharePostLink = "N/A",
                OriginalPostLink = "N/A"
            };

            try
            {
                if (timeList.Count == 1 && linkList.Count >= 1)
                {
                    // 🔸 Bài viết tự đăng                 
                    result.ShareTime = CleanTimeString(timeList[0]);
                    result.LinkBaiViet = ShearchPostDAO.Instance.ShortenFacebookPostLink(linkList[0]);
                }
                else if (timeList.Count == 2 && linkList.Count >= 2)
                {
                    // 🔹 Bài viết share                   
                    result.ShareTime = CleanTimeString(timeList[0]);
                    result.OriginalTime = CleanTimeString(timeList[1]);
                    result.SharePostLink = ShearchPostDAO.Instance.ShortenFacebookPostLink(linkList[0]);
                    result.OriginalPostLink = linkList[1];
                    result.LinkBaiViet = ShearchPostDAO.Instance.ShortenFacebookPostLink(linkList[0]);
                }
                else
                {
                    Console.WriteLine($"⚠️ Không xác định được loại bài: timeList.Count = {timeList.Count}, linkList.Count = {linkList.Count}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Lỗi nam trong PostTypeDetector: " + ex.Message);
            }

            return result;
        }
        public (List<string> timeList, List<string> linkList) ExtractTimeAndLinks(IEnumerable<IWebElement> postinfor)
        {
            List<string> timeList = new List<string>();
            List<string> linkList = new List<string>();
            HashSet<string> addedLinks = new HashSet<string>(); // Dùng để kiểm tra trùng link

            foreach (var temp in postinfor)
            {
                string textContent = temp.Text.Trim();
                // Kiểm tra có chứa từ ngữ liên quan đến thời gian không
                if (!string.IsNullOrEmpty(textContent) && Regex.IsMatch(textContent, @"(\d+\s*(giờ|phút|ngày|hôm qua|Tháng))", RegexOptions.IgnoreCase))
                {
                    var hrefElement = temp.FindElements(By.CssSelector("a[class*='x1i10hfl']"));
                    if (hrefElement.Count > 0)
                    {
                        // Duyệt từng href và lấy link chưa có
                        foreach (var linkEl in hrefElement)
                        {
                            string href = linkEl.GetAttribute("href");
                            if (!string.IsNullOrEmpty(href) && !addedLinks.Contains(href))
                            {
                                linkList.Add(href);
                                timeList.Add(textContent); // Thêm thời gian tương ứng với link
                                addedLinks.Add(href);                             
                            }
                            else
                            {
                                Console.WriteLine($"Link đã tồn tại hoặc rỗng: {href}");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Không có link trong phần tử này.");
                    }
                }
                else
                {
                    Console.WriteLine("Không thêm vì không chứa thông tin thời gian.");
                }
            }
            return (timeList, linkList);
        }
        public static string CleanTimeString(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return "N/A";

            // Loại bỏ ký tự xuống dòng, dấu chấm giữa (·), tab, nhiều khoảng trắng
            string cleaned = Regex.Replace(raw, @"[\n\r\t]+", " ");
            cleaned = Regex.Replace(cleaned, @"\s*·\s*", " "); // bỏ dấu ·
            cleaned = Regex.Replace(cleaned, @"\s{2,}", " "); // bỏ khoảng trắng dư
            return cleaned.Trim();
        }
        public (string, string, string) HandleVideoPost(IWebElement post) // hàm láy bài viết đăng video
        {
            string trangthai = "bài đăng có video";           
            string shareTime = "N/A";
            string linkBaiViet = "N/A";

            try
            {
                var videoLinks = post.FindElements(By.CssSelector("a[class ='x1i10hfl xjbqb8w x1ejq31n xd10rxx x1sy0etr x17r0tee x972fbf xcfux6l x1qhh985 xm0m39n x9f619 x1ypdohk xt0psk2 xe8uvvx xdj266r x11i5rnm xat24cr x1mh8g0r xexx8yu x4uap5 x18d9i69 xkhd6sd x16tdsg8 x1hl2dhg xggy1nq x1a2a7pz x1heor9g xkrqix3 x1sur9pj x1s688f']"));
                if (videoLinks.Count > 0)
                {
                    linkBaiViet = videoLinks[0].GetAttribute("href").ToString();
                    Console.WriteLine("🔗 Video Link: " + linkBaiViet);
                }

                var timeTags = post.FindElements(By.CssSelector("span[class = 'html-span xdj266r x11i5rnm xat24cr x1mh8g0r xexx8yu x4uap5 x18d9i69 xkhd6sd x1hl2dhg x16tdsg8 x1vvkbs x4k7w5x x1h91t0o x1h9r5lt x1jfb8zj xv2umb2 x1beo9mf xaigb6o x12ejxvf x3igimt xarpa2k xedcshv x1lytzrv x1t2pt76 x7ja8zs x1qrby5j']"));
                foreach (var t in timeTags)
                {
                    if (t.Text.Contains("phút") || t.Text.Contains("giờ") || t.Text.Contains("ngày"))
                    {
                        shareTime = t.Text.Trim();
                        Console.WriteLine("⏰ Video Time: " + shareTime);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Lỗi trong HandleVideoPost: " + ex.Message);
            }

            return (trangthai, shareTime, linkBaiViet);
        }
        
    }
}
