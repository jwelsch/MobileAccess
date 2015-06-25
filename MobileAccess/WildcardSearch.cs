using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileAccess
{
   //
   // Adapted from: https://github.com/max-kotasek/Wildcard
   //

   public static class WildcardSearch
   {
      public static Tuple<string, string> SplitPattern( string path )
      {
         var slash = path.LastIndexOf( '\\' );
         var wildcard = path.LastIndexOfAny( new char[] { '*', '?' } );

         if ( wildcard < 0 )
         {
            return new Tuple<string, string>( path, string.Empty );
         }

         if ( wildcard < slash )
         {
            throw new ArgumentException( "The wildcard can only be in the last section of the path." );
         }

         return new Tuple<string, string>( path.Substring( 0, slash ), path.Substring( slash + 1 ) );
      }

      private static int GetNextInterestingCharacter( string pattern )
      {
         var w = pattern.IndexOf( '*' );
         var q = pattern.IndexOf( '?' );

         if ( w == -1 && q == -1 )
         {
            return pattern.Length;
         }
         else if ( w == -1 && q != -1 )
         {
            return q;
         }
         else if ( w != -1 && q == -1 )
         {
            return w;
         }

         return Math.Min( w, q );
      }

      private static string LStrip( string pattern, string stripChars )
      {
         var index = 0;

         for ( var i = 0; i < pattern.Length; i++ )
         {
            var s = pattern[i].ToString();
            if ( stripChars.IndexOf( s ) >= 0 )
            {
               index++;
            }
            else
            {
               return pattern.Substring( index );
            }
         }

         // All characters were stripped.
         return string.Empty;
      }

      public static bool MatchOneOf( string pattern, Collection<String> inputs )
      {
         foreach ( var input in inputs )
         {
            if ( !WildcardSearch.Match( pattern, input ) )
            {
               continue;
            }

            return true;
         }

         return false;
      }

      public static bool Match( string pattern, string input )
      {
         if ( pattern == null || pattern.Length == 0 )
         {
            return false;
         }

         if ( input == null || input.Length == 0 )
         {
            return false;
         }

         var nextIndex = 0;
         var patternSlice = string.Empty;

         while ( pattern.Length > 0 )
         {
            if ( pattern[0] == '*' )
            {
               pattern = WildcardSearch.LStrip( pattern, "*?" );
               if ( pattern.Length == 0 )
               {
                  return true;
               }

               nextIndex = WildcardSearch.GetNextInterestingCharacter( pattern );
               patternSlice = pattern.Substring( 0, nextIndex );

               if ( input.IndexOf( patternSlice ) >= 0 )
               {
                  pattern = pattern.Substring( nextIndex );
                  var inputIndex = input.LastIndexOf( patternSlice );
                  input = input.Substring( inputIndex + patternSlice.Length );
               }
               else
               {
                  return false;
               }
            }

            else if ( pattern[0] == '?' )
            {
               if ( input.Length < 1 )
               {
                  return false;
               }
               pattern = pattern.Substring( 1 );
               input = input.Substring( 1 );
            }
            else
            {
               nextIndex = GetNextInterestingCharacter( pattern );
               patternSlice = pattern.Substring( 0, nextIndex );
               pattern = pattern.Substring( nextIndex );

               if ( input.Length < nextIndex )
               {
                  return false;
               }

               var matchSlice = input.Substring( 0, nextIndex );

               if ( !matchSlice.Equals( patternSlice ) )
               {
                  return false;
               }

               input = input.Substring( nextIndex );
            }

         }

         return ( input.Length == 0 );
      }
   }
}
