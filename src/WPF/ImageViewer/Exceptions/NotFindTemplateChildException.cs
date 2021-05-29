using System;

namespace ImageViewer
{
    public class NotFindTemplateChildException : Exception
    {
        public NotFindTemplateChildException(string childName)
            : base($"Child element {childName} not found in control theme")
        {
        }
    }
}