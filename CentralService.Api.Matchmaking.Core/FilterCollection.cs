using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralService.Api.Matchmaking.Core
{
    public class FilterCollection
    {
        public IReadOnlyCollection<Filter> Filters => _Filters;
        public IReadOnlyCollection<FilterCollection> SubCollections => _SubCollections;

        public List<Filter> _Filters = new List<Filter>();
        public List<FilterCollection> _SubCollections = new List<FilterCollection>();

        public FilterCollection(string FilterString)
        {
            int SubCollectionsStart = FilterString.IndexOf('(');
            if (SubCollectionsStart > 0)
            {
                ProcessFilters(FilterString.Substring(0, SubCollectionsStart - 5));
                ProcessSubCollections(FilterString.Substring(SubCollectionsStart, FilterString.Length - SubCollectionsStart));
            }
            else if (SubCollectionsStart == -1)
                ProcessFilters(FilterString);
            else
                ProcessSubCollections(FilterString);
        }

        public bool Run(Server Server)
        {
            foreach (Filter Filter in _Filters)
                if (!Filter.Run(Server))
                    return false;
            foreach (FilterCollection Collection in _SubCollections)
                if (Collection.RunOr(Server))
                    return false;
            return true;
        }

        private bool RunOr(Server Server)
        {
            List<bool> Results = new List<bool>();
            foreach (Filter Filter in _Filters)
                Results.Add(Filter.Run(Server));
            return Results.Contains(true);
        }

        private void ProcessSubCollections(string SubCollections)
        {
            string[] SplitSubCollections = ProcessSubCollectionsString(SubCollections);
            foreach (string SubCollection in SplitSubCollections)
            {
                if (SubCollection.Replace("OR", "or").Contains("or"))
                {
                    string[] SplitFilters = SubCollection.Split("or");
                    foreach (string Filter in SplitFilters)
                        _SubCollections.Add(new FilterCollection(Filter));
                }
                else
                    ProcessFilters(SubCollection);
            }
        }

        private string[] ProcessSubCollectionsString(string SubCollections)
        {
            string[] SplitFilter = SubCollections.Replace(" ", "").Replace("AND", "and").Split("and");

            int Difference = SplitFilter[0].Count(x => x == '(') - SplitFilter[0].Count(x => x == ')');
            if (Difference > 0)
                SplitFilter[0] = SplitFilter[0].Remove(0, Difference);

            int LastIndex = SplitFilter.Length - 1;
            Difference = SplitFilter[LastIndex].Count(x => x == ')') - SplitFilter[LastIndex].Count(x => x == '(');
            if (Difference > 0)
                SplitFilter[LastIndex] = SplitFilter[LastIndex].Remove(SplitFilter[LastIndex].Length - Difference, Difference);

            for (int i = 0; i < SplitFilter.Length; i++)
            {
                int SubCollectionLength = SplitFilter[i].Length;
                if (SplitFilter[i][0] == '(' && SplitFilter[i][SubCollectionLength - 1] == ')')
                    SplitFilter[i] = SplitFilter[i].Substring(1, SubCollectionLength - 2);
            }
            return SplitFilter;
        }

        private void ProcessFilters(string Filters)
        {
            string[] SplitFilters = Filters.Replace(" ", "").Replace("AND", "and").Split("or");
            for (int i = 0; i < SplitFilters.Length; i++)
            {
                string[] IndividualFilters = SplitFilters[i].Split("and");
                for (int j = 0; j < IndividualFilters.Length; j++)
                    _Filters.Add(new Filter(IndividualFilters[j]));
            }
        }
    }
}
