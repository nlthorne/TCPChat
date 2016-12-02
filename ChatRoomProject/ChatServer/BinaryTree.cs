using System;
using System.Net.Sockets;
using System.Collections;

namespace ChatRoomServer
{
    public class BinaryTree : IEnumerable
    {

        private Node top;
        private int _count = 0;

        public BinaryTree()
        {
            top = null;
            _count = 0;
        }


        public int Count()
        {
            return _count;
        }

        public Node Search(string name)
        {
            Node searchNode = top;
            while (searchNode != null)
            {
                if (String.Compare(name, searchNode.name) == 0)
                {
                    return searchNode;
                }

                if (String.Compare(name, searchNode.name) < 0)
                {
                    searchNode = searchNode.left;
                }
                else
                {
                    searchNode = searchNode.right;
                }
            }
            return null;
        }


        private void Add(Node node, ref Node tree)
        {
            if (tree == null)
                tree = node;
            else
            {
                if (String.Compare(node.name, tree.name) == 0)
                    throw new Exception();

                if (String.Compare(node.name, tree.name) < 0)
                {
                    Add(node, ref tree.left);
                }
                else
                {
                    Add(node, ref tree.right);
                }
            }
        }

        public Node Insert(string name, TcpClient client)
        {
            Node nodeToAdd = new Node(name, client);

            if (top == null)
            {
                top = nodeToAdd;
            }

            else
            {
                Add(nodeToAdd, ref top);
            }

            _count++;
            return nodeToAdd;

        }

        private Node LocateParent(string name, ref Node parent)
        {
            Node searchNode = top;
            parent = null;

            while (searchNode != null)
            {
                if (String.Compare(name, searchNode.name) == 0)
                    return searchNode;

                if (String.Compare(name, searchNode.name) < 0)
                {
                    parent = searchNode;
                    searchNode = searchNode.left;
                }
                else
                {
                    parent = searchNode;
                    searchNode = searchNode.right;
                }
            }
            return null;
        }

        public Node LocateChild(Node startNode, ref Node parent)
        {
            parent = startNode;
            startNode = startNode.right;
            while (startNode.left != null)
            {
                parent = startNode;
                startNode = startNode.left;
            }
            return startNode;
        }

        public void Delete(string key)
        {
            Node parent = null;
            // find the key and parent
            Node nodeToDelete = LocateParent(key, ref parent);
            if (nodeToDelete == null)
                throw new Exception("Unable to delete node: " + key.ToString());

            // no children
            if ((nodeToDelete.left == null) && (nodeToDelete.right == null))
            {
                if (parent == null)
                {
                    top = null;
                    return;
                }

                if (parent.left == nodeToDelete)
                    parent.left = null;
                else
                    parent.right = null;
                _count--;
                return;
            }

            //check right side, move child node up
            if (nodeToDelete.left == null)
            {
                // if at top
                if (parent == null)
                {
                    top = nodeToDelete.right;
                    return;
                }

                //parent gets new child
                if (parent.left == nodeToDelete)
                    parent.right = nodeToDelete.right;
                else
                    parent.left = nodeToDelete.right;
                nodeToDelete = null;
                _count--;
                return;
            }

            //check left side, move child up
            if (nodeToDelete.right == null)
            {
                //at top			
                if (parent == null)
                {
                    top = nodeToDelete.left;
                    return;
                }

                // parent gets new child
                if (parent.left == nodeToDelete)
                {
                    parent.left = nodeToDelete.left;
                }

                else
                {
                    parent.right = nodeToDelete.left;
                }

                nodeToDelete = null;
                _count--;
                return;
            }

            // Both children have children, 
            Node child = LocateChild(nodeToDelete, ref parent);

            Node childCopy = new Node(child.name, child.tcpClient);
            // Find out which side the child parent is pointing to the child and remove the child
            if (parent.left == child)
            {
                parent.left = null;
            }
            else
            {
                parent.right = null;
            }

            // move child into deleted node position
            nodeToDelete.name = childCopy.name;
            nodeToDelete.tcpClient = childCopy.tcpClient;
            _count--;
        }
        public IEnumerator GetEnumerator()
        {
            return top.GetEnumerator();
        }

    }
}
