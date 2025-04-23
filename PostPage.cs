using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrawFB.DTO
{
    public class PostPage
    {
        private string postTime;
        private string postLink;
        private string content;
        private string mediaType;
        private string shareCount;
        private string commentCount;
        private string posterName;
        private string posterlink;
        private string pageName;
        private string pageLink;
        private string orgiginalPostTime;
        private string orgiginalPostLink;
        private string orgiginalContent;
        private string originalPosterName;
        private string originalPosterLink;
        private string postStatus;
        public PostPage() { }
        public PostPage(string posttime, string postlink, string content, string sharecount, string commentcount, string postername, string posterlink, string pagename, string pagelink, string originaltime, string originallink, string originalpostername, string originalposterlink, string originalcontent, string poststatus)
        {
            this.PostTime= posttime;
            this.PostLink = postlink;
            this.Content = content;  
            this.ShareCount = sharecount;   
            this.CommentCount = commentcount;
            this.PosterName = postername;
            this.Posterlink = posterlink;
            this.pageName = pagename;
            this.pageLink = pagelink;
            this.orgiginalContent = originalcontent;
            this.orgiginalPostTime = originaltime;
            this.orgiginalPostLink = originallink;
            this.originalPosterName = originalpostername;
            this.originalPosterLink = originalposterlink;
            this.PostStatus = poststatus;
        }      
        public string Content { get => content; set => content = value; }     
        public string ShareCount { get => shareCount; set => shareCount = value; }
        public string CommentCount { get => commentCount; set => commentCount = value; }        
        public string PageName { get => pageName; set => pageName = value; }
        public string OrgiginalContent { get => orgiginalContent; set => orgiginalContent = value; }
        public string PageLink { get => pageLink; set => pageLink = value; }
        public string PosterName { get => posterName; set => posterName = value; }
        public string Posterlink { get => posterlink; set => posterlink = value; }
        public string OriginalPosterName { get => originalPosterName; set => originalPosterName = value; }
        public string OriginalPosterLink { get => originalPosterLink; set => originalPosterLink = value; }
        public string PostTime { get => postTime; set => postTime = value; }
        public string PostLink { get => postLink; set => postLink = value; }
        public string OrgiginalPostTime { get => orgiginalPostTime; set => orgiginalPostTime = value; }
        public string OrgiginalPostLink { get => orgiginalPostLink; set => orgiginalPostLink = value; }
        public string PostStatus { get => postStatus; set => postStatus = value; }
    }
}
