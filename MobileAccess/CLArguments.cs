using System;
using System.Collections.Generic;
using CommandLineLib;

namespace MobileAccess
{
   public class CLArguments
   {
      [FilePathCompound( "-source", Optional = true )]
      public string Source
      {
         get;
         private set;
      }
   }
}
