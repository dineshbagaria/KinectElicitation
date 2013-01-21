using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FaceTrackingBasics
{
    public enum ContentType { Movie, TVShow, Others }
    enum Genre {Action, Comedy,  Others }
    class Content
    {
        public ContentType contentType { set; get; }
        public Genre genre { set; get; }
        public int duration { set; get; }
        public String title { set; get; }
        public String leadActor { set; get; }
        public String summary { set; get; }

        public  Content(ContentType contentType ,Genre genre , int duration ,String title ,String leadActor,String summary )
        {
            this.contentType = contentType;
            this.genre = genre;
            this.duration = duration;
            this.title = title;
            this.leadActor = leadActor;
            this.summary = summary;
        }

        
    }
}

