using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebRole1.trie
{
    public enum NodeState
    {
        List,
        ParentNode
    }

    public class TrieNode
    {
        // pointer for the parent node
        public TrieNode ParentNode { get; set; }
        // specifies if the node denotes a complete word after its traversal
        public bool IsEndOfWord { get; set; }
        // list of words
        public List<Tuple<string, int>> WordList { get; set; }
        // contains character value of the node and the popcount
        public Tuple<char?, int> NodeValue { get; set; }
        // specifies if node functions as a list or as a parent of other child nodes
        public NodeState State { get; set; }
        // pointers to all of its child nodes
        public List<TrieNode> Children;

        public TrieNode()
        {

        }
                
        public TrieNode(TrieNode parentNode, bool isEndOfWord, char? nodeCharValue)
        {            
            ParentNode = parentNode;
            IsEndOfWord = isEndOfWord;
            WordList = new List<Tuple<string, int>>();
            NodeValue = new Tuple<char?, int>(nodeCharValue, 0);
            State = NodeState.List;
        }

        /// <summary>
        /// Inserts a word to a trie using this node as a starting point
        /// </summary>
        /// <param name="word"></param>
        public void InsertWord(Tuple<string, int> word)
        {
            // checks whether this node is a list or a parent node
            switch (State)
            {
                case NodeState.List: 
                    // adds the word to the list
                    WordList.Add(word);
                    // perform bursting if list count reached the threshold
                    if (WordList.Count() >= Trie.nodeLimitBeforeBursting)
                    {                       
                        Burst();
                    }
                    break;
                case NodeState.ParentNode:
                    // check if there is an existing node that matches the first character of the word to be inserted
                    var existingNode = Children.Select(x => x).Where(x => x.NodeValue.Item1 == word.Item1[0]);
                    if (existingNode.Count() > 0)
                    {
                        // do recursive call to this function while removing the first character of the word
                        existingNode.First().InsertWord(new Tuple<string, int>(word.Item1.Substring(1), word.Item2));
                    }
                    else
                    {
                        // create child node
                        TrieNode trieNode = new TrieNode(this, false, word.Item1[0]);
                        Children.Add(trieNode);
                        // insert this word to the child node
                        trieNode.InsertWord(new Tuple<string, int>(word.Item1.Substring(1), word.Item2));
                    }
                    
                    break;
            }
        }   
        
        /// <summary>
        /// happens when reaching max limit of list
        /// </summary>
        public void Burst()
        {
            // change node state
            State = NodeState.ParentNode;
            Children = new List<TrieNode>();
            // groups all words with the same first letter together     
            foreach (var wordGroup in WordList.Select(x => x).Where(x => x.Item1 != "").GroupBy(x => x.Item1[0]).Select(x => x))
            {                
                // creates a child node out the groups formed
                TrieNode trieNode = new TrieNode(this, false, wordGroup.Key);
                Children.Add(trieNode);
                // retrieves all words in the group and insert it to the added child node
                foreach(Tuple<string, int> word in wordGroup.ToList().Select(x => new Tuple<string, int>(x.Item1.Substring(1), x.Item2)))
                {                    
                    if (word.Item1 == "")
                    {
                        trieNode.IsEndOfWord = true;
                    }
                    else
                    {
                        trieNode.InsertWord(word);
                    }                    
                }
            }
            // remove all words in list
            WordList.Clear();
        }

        /// <summary>
        /// adds popularity count to this node or a word in the list of this node
        /// </summary>
        /// <param name="newPopCountValue"></param>
        /// <param name="keyWord">keyword used to match the appropriate word in the list</param>
        public void AddPopularity(int newPopCountValue, string keyWord)
        {            
            IEnumerable<Tuple<string, int>> targetWord = WordList.Select(x => x).Where(x => keyWord.ToLower().EndsWith(x.Item1.ToLower()));
            if (targetWord.Count() > 0)
            {                
                List<Tuple<string, int>> updateList = WordList.Select(x => x).Where(x => keyWord.ToLower().EndsWith(x.Item1.ToLower())).Select(x => x = new Tuple<string, int>(x.Item1, x.Item2 + newPopCountValue)).ToList();
                WordList.RemoveAll(x => keyWord.ToLower().EndsWith(x.Item1.ToLower()));
                WordList.AddRange(updateList);
            }
            else
            {
                NodeValue = new Tuple<char?, int>(NodeValue.Item1, NodeValue.Item2 + newPopCountValue);
            }  
        }

        /// <summary>
        /// gets popularity count of a selected keyword
        /// </summary>
        /// <param name="keyWord"></param>
        /// <returns></returns>
        public int GetPopularityOfEntry(string keyWord)
        {
            IEnumerable<Tuple<string, int>> targetWord = WordList.Select(x => x).Where(x => keyWord.ToLower().EndsWith(x.Item1.ToLower()));
            if (targetWord.Count() > 0)
            {
                List<Tuple<string, int>> updateList = WordList.Select(x => x).Where(x => keyWord.ToLower().EndsWith(x.Item1.ToLower())).ToList();
                return updateList.First().Item2;
            }
            else
            {
                return NodeValue.Item2;                
            }
        }
    }
}
