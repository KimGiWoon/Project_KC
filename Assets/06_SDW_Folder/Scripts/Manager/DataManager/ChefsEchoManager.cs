using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace SDW
{
    public class ChefsEchoManager : MonoBehaviour
    {
        //# ID(int) - ChefsEchoDataFileData
        private Dictionary<int, ChefsEchoDataFileData> _nodeIdData = new Dictionary<int, ChefsEchoDataFileData>();
        public Dictionary<int, ChefsEchoDataFileData> NodeIdData => _nodeIdData;

        //# NodeGrade(enum) - ChefsEchoDataFileData
        private Dictionary<NodeGrade, List<ChefsEchoDataFileData>> _nodeGradeData =
            new Dictionary<NodeGrade, List<ChefsEchoDataFileData>>();
        public Dictionary<NodeGrade, List<ChefsEchoDataFileData>> NodeGradeData => _nodeGradeData;

        //# NodeEnName(enum) - ChefsEchoDataFileData
        private Dictionary<NodeEnName, ChefsEchoDataFileData> _nodeEnNameData =
            new Dictionary<NodeEnName, ChefsEchoDataFileData>();
        public Dictionary<NodeEnName, ChefsEchoDataFileData> NodeEnName => _nodeEnNameData;

        private void Start()
        {
            LoadChefsEchoData();
        }

        private void LoadChefsEchoData()
        {
            string[] fields = HandleCSV.LoadFromCsv("Contents/ChefsEchoData");
            var chefsDataList = HandleCSV.ReadDataFromLines<ChefsEchoDataFileData>(fields);

            foreach (var chefsData in chefsDataList)
            {
                _nodeIdData[chefsData.NodeID] = chefsData;
                if (!_nodeGradeData.ContainsKey(chefsData.Grade))
                    _nodeGradeData[chefsData.Grade] = new List<ChefsEchoDataFileData>();
                _nodeGradeData[chefsData.Grade].Add(chefsData);

                _nodeEnNameData[chefsData.EnName] = chefsData;
            }
        }
    }
}