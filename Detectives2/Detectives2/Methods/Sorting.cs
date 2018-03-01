using Detectives2.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Detectives2.Methods
{
    public class Sorting
    {
        //I'm trying to create a generic method, instead of having copies of it in 3 different controllers.
        //The current problem is that the db column names don't seem to be accessible through the generic type
        //(though are fine with the non-generic version of the method).  I'll continue to work on this - maybe LINQ would do it?

        //public static IQueryable<T> GetDefault<T>()
        //{
        //    return default(T);
        //}

        //public static IQueryable<T> SortByLastName<T>(IQueryable<T> detectives, string sortOrder, FictionalDetectivesEntities1 db)
        //{
        //    string[] firstNames = new string[10];
        //    string[] lastNames = new string[10];
        //    int i = 0;
        //    int j = 0;

        //    //get the last word in each name string as the last name and make sure they're not spaces
        //    foreach (T d in detectives)
        //    {
        //        string[] temp = d.Name.Split(' ');
        //        //skip past spaces
        //        i = temp.Length - 1;
        //        while (temp[i] == " ")
        //        {
        //            i--;
        //        }
        //        //get last name - the last word in the string
        //        lastNames[j] = temp[i];
        //        j++;
        //    }
        //    //sort the array of last names in ascending or descending order
        //    if (sortOrder == "ascending")
        //    {
        //        Array.Sort(lastNames);
        //    }
        //    else if (sortOrder == "descending")
        //    {
        //        lastNames = lastNames.OrderByDescending(c => c).ToArray();
        //    }


        //    //get rid of null values by transferring actual values in the 
        //    //array into a list
        //    List<string> lastNamesList = new List<string>();
        //    for (int k = 0; k < lastNames.Length; k++)
        //    {
        //        if (lastNames[k] != null)
        //        {
        //            lastNamesList.Add(lastNames[k]);
        //        }
        //    }

        //    //Update db with sort order
        //    int sortIndex = 0;
        //    foreach (string lastName in lastNamesList)
        //    {
        //        foreach (T detective in detectives)
        //        {
        //            if (detective.Name.Contains(lastName))
        //            {
        //                detective.SortOrder = sortIndex;
        //                db.Entry(detective).State = EntityState.Modified;
        //            }
        //        }
        //        sortIndex++;
        //    }

        //    db.SaveChanges();

        //    detectives = detectives.OrderBy(x => x.SortOrder);

        //    return detectives;
        //}
    }
}