using System;
using CommandLineLib;

namespace MobileAccess
{
   class Program
   {
      static void Main( string[] args )
      {
         CommandLine<CLArguments> commandLine = null;

         try
         {
            commandLine = new CommandLine<CLArguments>();
            var arguments = commandLine.Parse( args );

            using ( var devices = WpdDeviceCollection.Create() )
            {
               if ( devices.Count == 0 )
               {
                  Console.WriteLine( "No devices found." );
               }
               else if ( arguments.CommandDiscover )
               {
                  foreach ( var device in devices )
                  {
                     Console.WriteLine( "\"{0}\"{1}", device.Name, arguments.ShowDeviceID ? " " + device.DeviceID : string.Empty );
                  }
               }
               else if ( arguments.CommandInfo )
               {
                  var device = devices.Find( arguments.InfoDeviceName, false );
                  if ( device == null )
                  {
                     Console.WriteLine( "No device found with the name \"{0}\".", arguments.InfoDeviceName );
                  }
                  else
                  {
                     if ( arguments.InfoCopyID )
                     {
                        ClipboardApi.Copy( device.DeviceID, true );
                     }

                     device.DisplayProperties();
                  }
               }
               else if ( arguments.CommandUpload )
               {
                  var device = devices.Find( arguments.UploadDeviceName, false );
                  if ( device == null )
                  {
                     Console.WriteLine( "No device found with the name \"{0}\".", arguments.UploadDeviceName );
                  }
                  else
                  {
                     Console.WriteLine( "Uploading..." );

                     var components = WildcardSearch.SplitPattern( arguments.UploadSourcePath );
                     var targetObject = device.ObjectFromPath( arguments.UploadTargetPath, arguments.CreatePath );

                     var commander = new DeviceCommander( device );
                     commander.DataCopyStarted += ( sender, e ) =>
                        {
                           Console.WriteLine( "\r{0}", e.SourcePath );
                        };
                     commander.DataCopied += ( sender, e ) =>
                        {
                           var percent = 100.0 * ( (double) e.CopiedBytes / (double) e.MaxBytes );
                           Console.Write( "\r  {0}/{1} bytes ({2}%)", e.CopiedBytes, e.MaxBytes, percent.ToString( "G3" ) );
                        };
                     commander.DataCopyEnded += ( sender, e ) =>
                        {
                        };
                     commander.DataCopyError += ( sender, e ) =>
                        {
                           Console.WriteLine( "\r" + e.Exception.Message );
                        };

                     Console.WriteLine();
                     if ( String.IsNullOrEmpty( components.Item2 ) )
                     {
                        commander.Upload( targetObject, components.Item1, arguments.Overwrite );
                     }
                     else
                     {
                        commander.Upload( targetObject, components.Item1, arguments.Overwrite, components.Item2, arguments.Recursive );
                     }
                     Console.WriteLine( "\rCompleted                                                                     " );
                  }
               }
               else if ( arguments.CommandDownload )
               {
                  var device = devices.Find( arguments.DownloadDeviceName, false );
                  if ( device == null )
                  {
                     Console.WriteLine( "No device found with the name \"{0}\".", arguments.DownloadDeviceName );
                  }
                  else
                  {
                     Console.WriteLine( "Downloading..." );

                     var components = WildcardSearch.SplitPattern( arguments.DownloadSourcePath );
                     var sourceObject = device.ObjectFromPath( components.Item1, false );

                     var commander = new DeviceCommander( device );
                     commander.DataCopyStarted += ( sender, e ) =>
                     {
                        Console.WriteLine( "\r{0}", e.SourcePath );
                     };
                     commander.DataCopied += ( sender, e ) =>
                     {
                        var percent = 100.0 * ( (double) e.CopiedBytes / (double) e.MaxBytes );
                        Console.Write( "\r  {0}/{1} bytes ({2}%)", e.CopiedBytes, e.MaxBytes, percent.ToString( "G3" ) );
                     };
                     commander.DataCopyEnded += ( sender, e ) =>
                     {
                     };
                     commander.DataCopyError += ( sender, e ) =>
                     {
                        Console.WriteLine( "\r" + e.Exception.Message );
                     };

                     Console.WriteLine();
                     if ( String.IsNullOrEmpty( components.Item2 ) )
                     {
                        commander.Download( sourceObject, arguments.DownloadTargetPath, arguments.Overwrite );
                     }
                     else
                     {
                        commander.Download( sourceObject, arguments.DownloadTargetPath, arguments.Overwrite, components.Item2, arguments.Recursive );
                     }
                     Console.WriteLine( "\rCompleted                                                                     " );
                  }
               }
               else
               {
                  Console.WriteLine( commandLine.Help() );
               }
            }
         }
         catch ( CommandLineDeclarationException ex )
         {
            if ( commandLine != null )
            {
               Console.WriteLine( commandLine.Help() );
            }
            System.Diagnostics.Trace.WriteLine( ex );
            Console.WriteLine( ex );
         }
         catch ( CommandLineException ex )
         {
            if ( commandLine != null )
            {
               Console.WriteLine( commandLine.Help() );
            }
            System.Diagnostics.Trace.WriteLine( ex );
            Console.WriteLine( ex );
         }
         catch ( Exception ex )
         {
            System.Diagnostics.Trace.WriteLine( ex );
            Console.WriteLine( ex );
         }
      }

      private static void WaitForKey( ConsoleKey key )
      {
         var keyInfo = new ConsoleKeyInfo();

         Console.WriteLine( "Press {0} to quit.", key );

         do
         {
            keyInfo = Console.ReadKey();
         }
         while ( keyInfo.Key != key );

         Console.WriteLine();
      }
   }
}
