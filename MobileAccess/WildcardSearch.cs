using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileAccess
{
   public static class WildcardSearch
   {


      private static int get_next_interesting_character( string pattern )
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
         else
         {
            return Math.Min( w, q );
         }

      }

      private static string lstrip( string pattern, string stripChars )
      {
         int index = 0;
         for ( int i = 0; i < pattern.Length; i++ )
         {
            string s = pattern[i].ToString();
            if ( stripChars.IndexOf( s ) >= 0 )
            {
               index++;
            }
            else
            {
               return pattern.Substring( index );
            }
         }
         //All characters were stripped.
         return "";
      }

      public static bool matchOneOf( string pattern, Collection<String> inputs )
      {
         foreach ( string input in inputs )
         {
            if ( !match( pattern, input ) )
            {
               continue;
            }
            return true;
         }
         return false;
      }

      public static bool match( string pattern, string input )
      {
         if ( pattern == null || pattern.Length == 0 )
         {
            return false;
         }

         if ( input == null || input.Length == 0 )
         {
            return false;
         }

         int next_index = 0;
         string pattern_slice = "";
         while ( pattern.Length > 0 )
         {

            if ( pattern[0] == '*' )
            {
               pattern = lstrip( pattern, "*?" );
               if ( pattern.Length == 0 )
               {
                  return true;
               }

               next_index = get_next_interesting_character( pattern );
               pattern_slice = pattern.Substring( 0, next_index );

               if ( input.IndexOf( pattern_slice ) >= 0 )
               {
                  pattern = pattern.Substring( next_index );
                  int input_index = input.LastIndexOf( pattern_slice );
                  input = input.Substring( input_index + pattern_slice.Length );
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
               next_index = get_next_interesting_character( pattern );
               pattern_slice = pattern.Substring( 0, next_index );
               pattern = pattern.Substring( next_index );

               if ( input.Length < next_index )
               {
                  return false;
               }

               string match_slice = input.Substring( 0, next_index );
               if ( !match_slice.Equals( pattern_slice ) )
               {
                  return false;
               }
               input = input.Substring( next_index );
            }

         }
         return ( input.Length == 0 );

      }
   }
}
