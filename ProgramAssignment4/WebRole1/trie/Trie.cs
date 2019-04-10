using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebRole1.trie
{
    public class Trie
    {
        // number of data stored in trie
        public static int dataStoredCount;
        // threshold limit for list in nodes
        public static int nodeLimitBeforeBursting = 200;

        // root node for the trie
        public TrieNode RootNode { get; }

        public Trie(string[] wikiList)
        {
            dataStoredCount = 0;
            RootNode = new TrieNode(null, false, null) ;
            ConstructTrie(wikiList, RootNode);            
        }

        /// <summary>
        /// inserts the word in the array to the trie
        /// </summary>
        /// <param name="listOfWords"></param>
        /// <param name="startingNode"></param>
        private static void ConstructTrie(string[] listOfWords, TrieNode startingNode)
        {
            StoreListOfWordsToNode(listOfWords, startingNode);
        }

        public static void StoreListOfWordsToNode(string[] listOfWords, TrieNode targetNode)
        {
            foreach (string word in listOfWords)
            {
                try
                {
                    targetNode.InsertWord(new Tuple<string, int>(word, 0));
                    dataStoredCount++;
                }
                catch(Exception e)
                {
                    TrieManager.ReportError(e.ToString());
                }
                   
            }
        }

        /// <summary>
        /// returns trienode with a specific match
        /// </summary>
        /// <param name="node"></param>
        /// <param name="prefixLetters"></param>
        /// <param name="keyWord"></param>
        /// <param name="isCaseSensitive"></param>
        /// <returns></returns>
        public static TrieNode GetSpecificEntry(TrieNode node, string prefixLetters, string keyWord, bool isCaseSensitive)
        {
            // returns this node if it is an end of a word and if the word formed during traversal to this node is equal to the target word
            if (node.IsEndOfWord && prefixLetters.ToLower() == keyWord.ToLower())
            {
                return node;
            }           

            switch (node.State)
            {
                case NodeState.List:
                    int matchCount;
                    // changes condition of linq depending if it is a case sensitive or case insensitive search
                    if (isCaseSensitive)
                    {
                        matchCount = node.WordList.Select(x => x).Where(x => prefixLetters + x.Item1 == keyWord).Count();
                    }
                    else
                    {
                        matchCount = node.WordList.Select(x => x).Where(x => (prefixLetters + x.Item1).ToLower() == keyWord.ToLower()).Count();
                    }
                    // returns the node if it has a member in the list that matches the target word
                    if (matchCount > 0)
                    {
                        return node;
                    }
                    else
                    {
                        return null;
                    }                    
                case NodeState.ParentNode:
                    IEnumerable<TrieNode> matchChild;
                    // changes condition of linq depending if it is a case sensitive or case insensitive search
                    if (isCaseSensitive)
                    {
                        matchChild = node.Children.Select(x => x).Where(x => keyWord.StartsWith(prefixLetters + x.NodeValue.Item1));
                    }
                    else
                    {
                        matchChild = node.Children.Select(x => x).Where(x => keyWord.ToLower().StartsWith((prefixLetters + x.NodeValue.Item1).ToLower()));
                    }
                    // call this function recursivley
                    if (matchChild.Count() > 0)
                    {
                        return GetSpecificEntry(matchChild.First(), prefixLetters + matchChild.First().NodeValue.Item1, keyWord, isCaseSensitive);
                    }
                    else
                    {
                        return null;
                    }
                default:
                    return null;
            }
        }

        /// <summary>
        /// gets all words from all the descendant nodes of the target node
        /// </summary>
        /// <param name="node"></param>
        /// <param name="prefixLetters">word formed during traversal</param>
        /// <param name="keyWord">searched word</param>
        /// <returns></returns>
        public static List<Tuple<string, int>> RetrieveAllWordsUnderANode(TrieNode node, string prefixLetters, string keyWord)
        {            
            List<Tuple<string, int>> listToReturn = new List<Tuple<string, int>>();

            if (node.IsEndOfWord && prefixLetters.ToLower().StartsWith(keyWord.ToLower()))
            {
                listToReturn.Add(new Tuple<string, int>(prefixLetters, node.NodeValue.Item2));
            }

            switch (node.State)
            {
                // gets all members of list that starts with the key word
                case NodeState.List:
                    listToReturn.AddRange(node.WordList.Select(x => new Tuple<string, int>(prefixLetters + x.Item1, x.Item2)).Where(x => x.Item1.ToLower().StartsWith(keyWord.ToLower())));
                    break;
                // recursively calls this function to each children of the node
                case NodeState.ParentNode:                    
                    foreach(TrieNode childNode in node.Children)
                    {
                        listToReturn.AddRange(RetrieveAllWordsUnderANode(childNode, (prefixLetters + childNode.NodeValue.Item1), keyWord));
                    }
                    break;
            }
            return listToReturn;
        }

        /// <summary>
        /// general method for traversing trie and getting specific data on the matching node
        /// </summary>
        /// <param name="prefixLetters"></param>
        /// <param name="stringToTraverse"></param>
        /// <param name="currentlySteppedNode"></param>
        /// <param name="isCaseSensitive"></param>
        /// <returns></returns>
        public static Tuple<TrieNode, string> GetNodeAfterTraversingString(string prefixLetters, string stringToTraverse, TrieNode currentlySteppedNode, bool isCaseSensitive)
        {
            // signifies that the node is the end of the traversal and returns the values of this node
            if (stringToTraverse == "")
            {
                return new Tuple<TrieNode, string>(currentlySteppedNode, prefixLetters + currentlySteppedNode.NodeValue.Item1);
            }

            switch (currentlySteppedNode.State)
            {
                // searches the list for the word that starts with the searched word
                case NodeState.List:
                    int existingMatchesCount;
                    if (isCaseSensitive)
                    {
                        existingMatchesCount = currentlySteppedNode.WordList.Select(x => x.Item1).Where(x => x.StartsWith(stringToTraverse)).Count();
                    }
                    else
                    {
                        existingMatchesCount = currentlySteppedNode.WordList.Select(x => x.Item1).Where(x => x.ToLower().StartsWith(stringToTraverse.ToLower())).Count();
                    }

                    if (existingMatchesCount > 0)
                    {
                        return new Tuple<TrieNode, string>(currentlySteppedNode, prefixLetters + currentlySteppedNode.NodeValue.Item1);
                    }
                    else
                    {
                        return null;
                    }    
                // finds a child node that matches the first character of the searched word
                case NodeState.ParentNode:
                    IEnumerable<TrieNode> existingChild;
                    if (isCaseSensitive)
                    {
                        existingChild = currentlySteppedNode.Children.Select(x => x).Where(x => x.NodeValue.Item1 == stringToTraverse[0]);
                    }
                    else
                    {
                        existingChild = currentlySteppedNode.Children.Select(x => x).Where(x => x.NodeValue.Item1.ToString().ToLower() == stringToTraverse[0].ToString().ToLower());
                    }                    
                    if (existingChild.Count() > 0)
                    {
                        // recursively call this function
                        return GetNodeAfterTraversingString(prefixLetters + currentlySteppedNode.NodeValue.Item1, stringToTraverse.Substring(1), existingChild.First(), isCaseSensitive);
                    }
                    else
                    {
                        return null;
                    }                    
                    
                default:
                    return null;
            }
        }   
    }
}
