﻿using PortableDeviceApiLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileAccess
{
   public interface IWpdDeviceObject
   {
      string ObjectID
      {
         get;
      }

      IWpdDeviceObject Parent
      {
         get;
      }

      IPortableDeviceContent Content
      {
         get;
      }

      string Name
      {
         get;
      }

      IWpdDeviceObject[] GetChildren();
      string GetPath();
   }
}