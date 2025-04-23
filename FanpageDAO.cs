using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CrawFB.DTO;
using OpenQA.Selenium;

namespace CrawFB.DAO
{
    internal class FanpageDAO
    {
        public static FanpageDAO instance;
        public static FanpageDAO Instance
        {
            get { if (instance == null) instance = new FanpageDAO(); return FanpageDAO.instance; }
            private set { FanpageDAO.instance = value; }

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
                        }
                    }
                }
            }
            return (timeList, linkList);
        }
        public (string PostTime, string PostLink) ExtractTimeAndLinksReelsPost(IWebElement post)
        {
            string posttime = string.Empty;
            string postlink = string.Empty;
            var PostReelsTime = post.FindElements(By.CssSelector("div[class='x9f619 x1n2onr6 x1ja2u2z x78zum5 xdt5ytf x2lah0s x193iq5w xl56j7k xurb0ha x1sxyh0 x1gslohp x12nagc xzboxd6 x14l7nz5']>span>span"));
            foreach (var PostReel in PostReelsTime)
            {
                posttime = CleanTimeString(PostReel.Text);

            }
            var PostReelsLink = post.FindElements(By.CssSelector("div[class='xal61yo xfrllxf xjppbhk x1k1lpwg x5omr3n x78zum5 x1q0g3np x1egiwwb xl56j7k x6ikm8r x10wlt62 x1n2onr6 xh8yej3 x16zosiy x1db0b2 x3awd8m']>a"));
            foreach (var Postreellink in PostReelsLink)
            {
                postlink = Postreellink.GetAttribute("href");      
                postlink = ShortenFacebookPostLink(postlink);
            }
            return (posttime, postlink);
        }
        public string CleanTimeString(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return "N/A";

            // Loại bỏ ký tự xuống dòng, dấu chấm giữa (·), tab, nhiều khoảng trắng
            string cleaned = Regex.Replace(raw, @"[\n\r\t]+", " ");
            cleaned = Regex.Replace(cleaned, @"\s*·\s*", " "); // bỏ dấu ·
            cleaned = Regex.Replace(cleaned, @"\s{2,}", " "); // bỏ khoảng trắng dư
            cleaned = Regex.Replace(cleaned, @"^[^\d]*", "");
            return cleaned.Trim();
        }
        public IWebElement GetSafe(IList<IWebElement> list, int index)
        {
            if (index < list.Count) return list[index];
            throw new ArgumentOutOfRangeException($"postinfor không có phần tử thứ {index}.");
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
                int indexreels = originalLink.IndexOf("?s=");
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
                else if(indexreels != -1)
                { 
                    cutIndex = indexreels;
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
        public string GetFullPostContent(IWebDriver driver, IReadOnlyCollection<IWebElement> contentElements)
        {
            string fullContent = "";

            foreach (var content in contentElements)
            {
                try
                {
                    if (content.Text.Contains("Xem thêm"))
                    {
                        var seeMoreButton = content.FindElement(By.CssSelector("div[class='x1i10hfl xjbqb8w x1ejq31n xd10rxx x1sy0etr x17r0tee x972fbf xcfux6l x1qhh985 xm0m39n x9f619 x1ypdohk xt0psk2 xe8uvvx xdj266r x11i5rnm xat24cr x1mh8g0r xexx8yu x4uap5 x18d9i69 xkhd6sd x16tdsg8 x1hl2dhg xggy1nq x1a2a7pz xkrqix3 x1sur9pj xzsf02u x1s688f']"));

                        if (seeMoreButton != null)
                        {
                            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", seeMoreButton);
                            Libary.Instance.randomtime(1000, 2000);
                            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", seeMoreButton);
                            Libary.Instance.randomtime(3000, 5000);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Lỗi khi xử lý 'Xem thêm': " + ex.Message);
                }
                // Luôn thu thập nội dung (kể cả nếu click lỗi)
                fullContent += content.Text.Trim() + "\n";
            }

            return fullContent.Trim();
        }
        public (string name, string link) GetPosterInfoBySelectors(IWebElement container)
        {
            var el = container.FindElements(By.CssSelector("span[class='xjp7ctv'] > a"));
            if (el.Count > 0) return GetPosterInfo(el);

            var el2 = container.FindElements(By.CssSelector("span[class='xjp7ctv'] > span > span > a"));
            if (el2.Count > 0) return GetPosterInfo(el2);

            throw new Exception("Không tìm thấy thẻ chứa thông tin người đăng.");
        }
        public (string PosterName, string PosterLink) GetPosterInfo(IReadOnlyCollection<IWebElement> userElements)
        {
            if (userElements == null || userElements.Count == 0)
                throw new Exception("Danh sách userElements rỗng, không thể lấy người đăng.");

            var firstUser = userElements.First();
            var PosterName = firstUser.Text.Trim();
            var PosterLink = ExtractFbShortLink(firstUser.GetAttribute("href"));

            return (PosterName, PosterLink);
        }
        public string ExtractFbShortLink(string url)
        {
            // ✅ 1. Kiểm tra dạng /user/1234567890
            var matchUserId = Regex.Match(url, @"/user/(\d+)");
            if (matchUserId.Success)
            {
                return $"https://fb.com/{matchUserId.Groups[1].Value}";
            }

            // ✅ 2. Kiểm tra dạng profile.php?id=1234567890
            var matchId = Regex.Match(url, @"[?&]id=(\d+)");
            if (matchId.Success)
            {
                return $"https://fb.com/{matchId.Groups[1].Value}";
            }

            // ✅ 3. Lấy tên người dùng từ link: facebook.com/username
            var matchShortName = Regex.Match(url, @"facebook\.com/([^/?&]+)");
            if (matchShortName.Success)
            {
                return $"https://fb.com/{matchShortName.Groups[1].Value}";
            }

            // ❌ Không phù hợp định dạng nào
            return "Invalid URL";
        }
        public string GetContentText(IWebDriver driver, IWebElement container)
        {
            var contentEls = container.FindElements(By.CssSelector(
                "span[class='x193iq5w xeuugli x13faqbe x1vvkbs x1xmvt09 x1lliihq x1s928wv xhkezso x1gmr53x x1cpjm7i x1fgarty x1943h6x xudqn12 x3x7a5m x6prxxf xvq8zen xo1l8bm xzsf02u x1yc453h']"));

            if (contentEls.Count == 0)
                throw new Exception("Không tìm thấy nội dung bài đăng.");

            return GetFullPostContent(driver, contentEls);
        }

        public string GetBackgroundText(IWebElement post)
        {
            var bgElements = post.FindElements(By.CssSelector("div[class='xdj266r x11i5rnm xat24cr x1mh8g0r x1vvkbs']"));
            return string.Join("\n", bgElements.Select(e => e.Text.Trim()).Where(t => !string.IsNullOrEmpty(t)));
        }
        public (string postTime, string originalPostTime, string postLink, string originalPostLink) PostTypeDetector(List<string> timeList, List<string> linkList)
        {
            string postTime = "N/A";
            string originalPostTime = "N/A";
            string postLink = "N/A";
            string originalPostLink = "N/A";

            try
            {
                int timeCount = timeList.Count;
                int linkCount = linkList.Count;

                if (timeCount == 1 && linkCount >= 1)
                {
                    // 🔸 Bài viết tự đăng
                    postTime = CleanTimeString(timeList[0]);
                    postLink = ShearchPostDAO.Instance.ShortenFacebookPostLink(linkList[0]);
                }
                else if (timeCount == 2 && linkCount >= 2)
                {
                    // 🔹 Bài viết share
                    postTime = CleanTimeString(timeList[0]);
                    originalPostTime = CleanTimeString(timeList[1]);
                    postLink = ShearchPostDAO.Instance.ShortenFacebookPostLink(linkList[0]);
                    originalPostLink = linkList[1];
                }
                else if (timeCount == 0 && linkCount == 0)
                {
                    Console.WriteLine("⚠️ Đây có thể là bài video, không có thời gian và link trong danh sách.");
                }
                else
                {
                    Console.WriteLine($"⚠️ Không xác định được loại bài: timeList.Count = {timeCount}, linkList.Count = {linkCount}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Lỗi trong PostTypeDetector: " + ex.Message);
            }

            return (postTime, originalPostTime, postLink, originalPostLink);
        }
        public PostPage GetFanpagePost(IWebDriver driver, IWebElement post)
        {
            string ShareCount = "N/A", CommentCount = "N/A", trangthai = "N/A";
            string PosterName = "N/A", PosterLink = "N/A", PageName = "", PageLink = "";
            string OriginalContent = "", fullcontent = "", OriginalPosterName = "", OriginalPosterLink = "";
            string PostTime = "", OriginalPostTime = "", PostLink = "", OriginalPostLink = "";

            var postinfor = post.FindElements(By.CssSelector("div[class='xu06os2 x1ok221b']"));
            var timeList = new List<string>();
            var linkList = new List<string>();
            if (postinfor.Count == 0)
            {
                (PostTime, PostLink) = FanpageDAO.Instance.ExtractTimeAndLinksReelsPost(post);
                var ShareCommentElementReels = post.FindElements(By.CssSelector("div[class = 'x9f619 x1n2onr6 x1ja2u2z x78zum5 xdt5ytf x2lah0s x193iq5w x6s0dn4 x1gslohp x12nagc xzboxd6 x14l7nz5']>span>span"));
                if(ShareCommentElementReels.Count >0)
                {
                    CommentCount = ShareCommentElementReels[1].Text;
                    ShareCount = ShareCommentElementReels[2].Text;
                }  
                else
                {
                    Console.WriteLine("không lấy được share/comment Bài Reels");
                }
            }
            else
            {
                (timeList, linkList) = FanpageDAO.Instance.ExtractTimeAndLinks(postinfor);
                (PostTime, OriginalPostTime, PostLink, OriginalPostLink) = FanpageDAO.Instance.PostTypeDetector(timeList, linkList);
                try
                {
                    var ShareCommentElement = post.FindElements(By.CssSelector("div[class='x9f619 x1n2onr6 x1ja2u2z x78zum5 xdt5ytf x2lah0s x193iq5w xeuugli xsyo7zv x16hj40l x10b6aqq x1yrsyyn']"));
                    foreach (var el in ShareCommentElement)
                    {
                        string text = el.Text;
                        if (text.Contains("bình luận")) CommentCount = text;
                        else ShareCount = text;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("❌ Lỗi lấy số chia sẻ/bình luận: " + ex.Message);
                }
            }

            if (string.IsNullOrEmpty(PostLink)) return null;

            try
            {
                switch (timeList.Count)
                {
                    case 1:
                        if (postinfor.Count == 3 || postinfor.Count == 2)
                        {
                            var posterContainer = FanpageDAO.Instance.GetSafe(postinfor, 0);
                            (PosterName, PosterLink) = FanpageDAO.Instance.GetPosterInfoBySelectors(posterContainer);

                            if (postinfor.Count == 3)
                            {
                                var contentContainer = FanpageDAO.Instance.GetSafe(postinfor, 2);
                                fullcontent = FanpageDAO.Instance.GetContentText(driver, contentContainer);
                                trangthai = "bài đăng có nội dung";
                            }
                            else
                            {
                                fullcontent = FanpageDAO.Instance.GetBackgroundText(post);
                                trangthai = "bài đăng cá nhân nền màu";
                            }
                        }
                        break;

                    case 2:
                        switch (postinfor.Count)
                        {
                            case 6:
                                {
                                    var posterContainer = FanpageDAO.Instance.GetSafe(postinfor, 0);
                                    var contentContainer = FanpageDAO.Instance.GetSafe(postinfor, 2);
                                    var originalPosterContainer = FanpageDAO.Instance.GetSafe(postinfor, 3);
                                    var originalContentContainer = FanpageDAO.Instance.GetSafe(postinfor, 5);

                                    (PosterName, PosterLink) = FanpageDAO.Instance.GetPosterInfoBySelectors(posterContainer);
                                    (OriginalPosterName, OriginalPosterLink) = FanpageDAO.Instance.GetPosterInfoBySelectors(originalPosterContainer);

                                    fullcontent = FanpageDAO.Instance.GetContentText(driver, contentContainer);
                                    OriginalContent = FanpageDAO.Instance.GetContentText(driver, originalContentContainer);

                                    trangthai = "bài share lại bài cá nhân, page";
                                }
                                break;

                            case 5:
                                {
                                    var posterContainer = FanpageDAO.Instance.GetSafe(postinfor, 0);
                                    (PosterName, PosterLink) = FanpageDAO.Instance.GetPosterInfoBySelectors(posterContainer);
                                    int contentIndex = -1;
                                    int originalPosterIndex = -1;
                                    // lấy original poster
                                    try
                                    {
                                        var originalPosterContainer = GetSafe(postinfor, 2);
                                        (OriginalPosterName, OriginalPosterLink) = GetPosterInfoBySelectors(originalPosterContainer);
                                        originalPosterIndex = 2;
                                    }
                                    catch
                                    {
                                        var originalPosterContainer = GetSafe(postinfor, 3);
                                        (OriginalPosterName, OriginalPosterLink) = GetPosterInfoBySelectors(originalPosterContainer);
                                        originalPosterIndex = 3;
                                    }
                                    // lấy content
                                    if (originalPosterIndex == 2) contentIndex = 4;
                                    else if (originalPosterIndex == 3) contentIndex = 2;
                                    if (contentIndex == 2)
                                    {
                                        var ContentContainer = GetSafe(postinfor, contentIndex);
                                        fullcontent = GetContentText(driver, ContentContainer);
                                        OriginalContent = GetBackgroundText(post);
                                    }
                                    else if (contentIndex == 4)
                                    {
                                        var originalContentContainer = GetSafe(postinfor, contentIndex);
                                        OriginalContent = GetContentText(driver, originalContentContainer);
                                        fullcontent = GetBackgroundText(post);
                                    }
                                    Console.WriteLine($"[DEBUG] originalPosterIndex = {originalPosterIndex}, contentIndex = {contentIndex}");
                                    Console.WriteLine("full" + fullcontent);
                                    Console.WriteLine("orinal" + OriginalContent);
                                    trangthai = "bài share lại thiếu 1 content";

                                }
                                break;
                            case 4:
                                {
                                    Console.WriteLine("vào case = 4");
                                    var posterContainer = FanpageDAO.Instance.GetSafe(postinfor, 0);
                                    (PosterName, PosterLink) = FanpageDAO.Instance.GetPosterInfoBySelectors(posterContainer);
                                    var originalPosterContainer = FanpageDAO.Instance.GetSafe(postinfor, 2);
                                    (OriginalPosterName, OriginalPosterLink) = FanpageDAO.Instance.GetPosterInfoBySelectors(originalPosterContainer);
                                    fullcontent = GetBackgroundText(post);
                                    trangthai = "share lai bai co nen mau";
                                }
                                break;
                        }
                        break;

                    case 0:
                        if (postinfor.Count == 0)
                        {
                            trangthai = "có thể là bài Reels";
                        }
                        break;
                }                                          
                return new PostPage(PostTime, PostLink, fullcontent, ShareCount, CommentCount,
                    PosterName, PosterLink, PageName, PageLink, OriginalPostTime, OriginalPostLink,
                    OriginalPosterName, OriginalPosterLink, OriginalContent, trangthai);
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Lỗi tổng khi phân tích bài viết: " + ex.Message);
                return null;
            }
        }
    }
}
