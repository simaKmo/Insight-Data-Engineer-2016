using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Specialized;

namespace Insight_DataEngineer_2016.src
{
    public class SlidingWindow
    {
        /// <summary>
        /// my base algoriths is base of the edges!
        /// it means that for every two tags, there is a possible edges in graph (if they are in same tweet)
        /// so I build all possible edges by using all tags which are in same tweets
        /// each edges is cunstructing uniquely by cantacting the name of two tags
        /// for example if one tag is Appache and the other one is Spark the edge name is AppacheSpark
        /// I am using dictionary to keep edges uniquely, but we need to now how mant same edges are added by each tweet
        /// 
        /// possible another possible solusion is using graph or tree but becuse dictionary has better time accessibility I prefer to use dictionary
        /// </summary>

        private DateTime _minDate;

        private SortedSet<DateTime> _dates;
        private Dictionary<DateTime, List<string>> _datedEdges;// keeping  all edges for a specific time
        private Dictionary<string, int> _edgesList; // keeping the number of each edges. all edge are unique but maybe two tweet has two same edges
        private Dictionary<string, int> _nodesList; // keeping the number of each node (tags)
        private Dictionary<string, List<string>> _edgesNodes; //keeping the tags of each edge



        public SlidingWindow()
        {
            _minDate = DateTime.MinValue;
            _dates = new SortedSet<DateTime>();
            _datedEdges = new Dictionary<DateTime, List<string>>();
            _edgesList = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);
            _nodesList = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);
            _edgesNodes = new Dictionary<string, List<string>>(StringComparer.InvariantCultureIgnoreCase);
        }
        /// <summary>
        /// this method check the sliding windows for possible updates:
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="date"></param>
        public void CheckSlidingWindowUpdate(List<string> tags, string date)
        {
            Dictionary<string, List<string>> edges;
            try
            {
                if (_minDate == DateTime.MinValue)
                {
                    _minDate = DateTime.ParseExact(date, "ddd MMM dd HH:mm:ss zzzz yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    _dates.Add(_minDate);

                    if (tags != null && tags.Count > 1)
                    {
                        edges = GetEdges(tags);
                        // if the hashtag is only includes single node
                        if (edges == null || edges.Count == 0)
                            return;
                        foreach (string item in tags)
                            if (!_nodesList.ContainsKey(item))
                                _nodesList.Add(item, 1);
                            else _nodesList[item]++;

                        List<string> values = new List<string>();


                        foreach (string item in edges.Keys)
                        {
                            _edgesList.Add(item, 1);
                            values.Add(item);
                            _edgesNodes.Add(item, edges[item]);
                        }
                        _datedEdges.Add(_minDate, values);
                        edges.Clear();
                        edges = null;
                    }
                }
                else
                    Update(date, tags);
            }
            catch (Exception e)
            {

                throw e;
            }

        }
        /// <summary>
        /// this method includes two parts:
        /// first removing all nodes that already are not in last 60 seconds window
        /// second add new tags 
        ///
        /// </summary>
        /// <param name="date"></param>
        /// <param name="tags"></param>
        private void Update(string date, List<string> tags)
        {

            try
            {
                DateTime now = DateTime.ParseExact(date, "ddd MMM dd HH:mm:ss zzzz yyyy", System.Globalization.CultureInfo.InvariantCulture);

                DateTime first = _dates.First();
                DateTime last = _dates.Last();
                #region Remove
                if ((last - now).TotalSeconds > 60)
                    return;
                while ((now - first).TotalSeconds > 60)
                {

                    List<string> keys;
                    if (_datedEdges.ContainsKey(first))
                    {
                        keys = _datedEdges[first];
                        _datedEdges.Remove(first);

                        try
                        {


                            foreach (string item in keys)
                            {
                                if (_edgesList[item] > 1)
                                    _edgesList[item]--;
                                else
                                    _edgesList.Remove(item);

                                List<string> nodes = _edgesNodes[item];
                                foreach (string n in nodes)
                                {
                                    if (_nodesList[n] > 1)
                                        _nodesList[n]--;
                                    else _nodesList.Remove(n);
                                }
                            }
                        }
                        catch (Exception e)
                        {

                            throw e;
                        }
                    }

                    _dates.Remove(_dates.First());
                    if (_dates.Count > 0)
                        first = _dates.First();
                    else
                        break;
                }


                #endregion Remove

                _dates.Add(now);
                _minDate = _dates.First();

                #region Add
                if (tags != null && tags.Count > 1)
                {
                    Dictionary<string, List<string>> edges = GetEdges(tags);

                    if (!_datedEdges.ContainsKey(now))
                        _datedEdges.Add(now, edges.Keys.ToList());
                    else
                    {
                        _datedEdges[now].AddRange(edges.Keys.ToList());
                    }
                    foreach (string str in edges.Keys)
                    {

                        if (!_edgesList.ContainsKey(str))
                            _edgesList.Add(str, 1);
                        else ++_edgesList[str];

                        foreach (string n in edges[str])
                        {
                            if (!_nodesList.ContainsKey(n))
                                _nodesList.Add(n, 1);
                            else ++_nodesList[n];
                        }

                        if (!_edgesNodes.ContainsKey(str))
                            _edgesNodes.Add(str, edges[str]);
                    }

                }
                #endregion ADD
            }
            catch (Exception e)
            {

                throw e;
            }

        }
        /// <summary>
        /// returning all possible edges by considering new tags
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        private Dictionary<string, List<string>> GetEdges(List<string> tags)
        {
            Dictionary<string, List<string>> edges = new Dictionary<string, List<string>>();
            int size = tags.Count;
            for (int i = 0; i < size; i++)
                for (int j = i + 1; j < size; j++)
                {
                    string str1 = tags[i];
                    string str2 = tags[j];
                    if (str1.CompareTo(str2) < 0)
                    {
                        string temp = str1;
                        str1 = str2;
                        str2 = temp;

                    }
                    string key = str1 + str2;
                    if (!edges.ContainsKey(key))
                    {
                        List<string> nodes = new List<string>();
                        nodes.Add(str1);
                        nodes.Add(str2);
                        edges.Add(key, nodes);
                    }
                }
            return edges;

        }
        /// <summary>
        /// calculating the degree of new sliding window (tweet graph)
        /// </summary>
        /// <returns></returns>
        public double GetDegree()
        {
            int size = _edgesList.Count;
            if (size == 0) return 0;
            double res = Double.Parse((Double.Parse((size * 2).ToString()) / (_nodesList.Count)).ToString());
            res = Math.Truncate(res * 100) / 100;
            return res;
        }




    }
}
