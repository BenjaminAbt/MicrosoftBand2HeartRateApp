using System;

namespace BandApp.Services
{
    public class NoBandFoundException : Exception
    {
        public NoBandFoundException() : base( "No band found." )
        {

        }
    }
}
