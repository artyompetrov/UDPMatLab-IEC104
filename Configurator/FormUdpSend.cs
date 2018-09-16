using SimulinkIEC104;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Configurator
{
    public partial class FormUdp : Form
    {
        BindingList<Destination> _dest;
        List<TreeNode> nodes = new List<TreeNode>();
        IEC104ReceiveParameter _iec104ReceiveP;
        IEC104SendParameter _iec104SendP;
        TreeNode _checkedNode;
        bool _receive;



        public FormUdp(BindingList<Destination> destinations, IEC104ReceiveParameter iec104ReceiveP)
        {
            InitializeComponent();
            _receive = true;

            _dest = destinations;
            _iec104ReceiveP = iec104ReceiveP;
            treeView1.CheckBoxes = true;

            foreach (var dest in _dest )
            {
                HiddenCheckBoxTreeNode node = new HiddenCheckBoxTreeNode();
                treeView1.Nodes.Add(node);
                node.Text = dest.ToString();
                node.Expand();
                foreach (var sendParam in dest.SendingParameters)
                {
                    if (sendParam.SourceParameter == null || sendParam.SourceParameter == iec104ReceiveP)
                    {
                        TreeNode subNode = new TreeNode();
                        node.Nodes.Add(subNode);
                        subNode.Text = sendParam.ToString();
                        subNode.Tag = sendParam;
                        nodes.Add(subNode);

                        if (_iec104ReceiveP.UDPparameters.Contains(sendParam))
                        {

                            subNode.Checked = true;
                        }
                    }
                }
            }
        }


        public FormUdp(BindingList<Destination> destinations, IEC104SendParameter iec104SendP)
        {
            InitializeComponent();

            _dest = destinations;
            _iec104SendP = iec104SendP;
            treeView1.CheckBoxes = true;
            foreach (var dest in _dest)
            {
                HiddenCheckBoxTreeNode node = new HiddenCheckBoxTreeNode();
                treeView1.Nodes.Add(node);
                node.Text = dest.ToString();
                
                node.Expand();
                foreach (var receiveParam in dest.ReceivingParameters)
                {
                    TreeNode subNode = new TreeNode();
                    node.Nodes.Add(subNode);
                    subNode.Text = receiveParam.ToString();
                    subNode.Tag = receiveParam;
                    nodes.Add(subNode);
                    if (_iec104SendP.UDPParameter == receiveParam)
                    {
                        subNode.Checked = true;
                        _checkedNode = subNode;

                    }

                }
            }
        }

        

        private void FormUdpSend_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_receive)
            {
                _iec104ReceiveP.ClearUDPParameter();
                foreach (var node in nodes)
                {
                    if (node.Checked)
                        _iec104ReceiveP.AddUDPparameter((SendingParameter)node.Tag);

                }
            }
            else
            {
                _iec104SendP.ClearUDPParameter();
                foreach (var node in nodes)
                {                    
                    if (node.Checked)
                        _iec104SendP.SetUDPParameter((ReceivingParameter)node.Tag);
                }
            }

            
        }

        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (!_receive)
            {
                if (_checkedNode != null && e.Node != _checkedNode)
                {
                    _checkedNode.Checked = false;
                    
                }
                _checkedNode = e.Node;
            }
        }
    }
}
