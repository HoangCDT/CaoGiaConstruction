using CaoGiaConstruction.WebClient.AutoMapper.ViewModels;
using CaoGiaConstruction.WebClient.Context.Entities;

namespace CaoGiaConstruction.WebClient.ViewModels
{
    public class AboutViewModel
    {
        public About AboutSettings { get; set; } // Existing generic content
        public TeamMember Founder { get; set; }
        public List<TeamMember> TeamMembers { get; set; }
        public List<TimeLine> Milestones { get; set; }
        public List<CoreValue> CoreValues { get; set; }
        public List<Slide> Partners { get; set; }
        public SlideVM AboutSlide { get; set; } // Slide for about page banner/image
    }
}
