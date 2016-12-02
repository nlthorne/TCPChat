using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Collections;

namespace ChatRoomServer
{
    public class Node : IEnumerable
    {
        public string name;
        public TcpClient tcpClient;
        public Node left;
        public Node right;

        public Node(string name, TcpClient client)
        {
            this.name = name;
            tcpClient = client;
            this.left = null;
            this.right = null;
        }

        public IEnumerator GetEnumerator()
        {
            if (left != null)
            {
                foreach (var node in left)
                {
                    yield return node;
                }
            }
            yield return this;

            if (right != null)
            {
                foreach (var node in right)
                {
                    yield return node;
                }
            }

        }
    }

}
