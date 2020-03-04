using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HOSystem
{
    [Serializable]
    public class HO_LocationInfo:IHOData
    {
        [SerializeField]
        private string location;
        [SerializeField]
        private string rank;
        [SerializeField]
        private float offset;
        [SerializeField]
        private HOMode mode;
        [SerializeField]
        private List<HOItem> reward;
        [SerializeField]
        private Dictionary<string, string> settings;

        public string Location
        {
            get
            {
                return location;
            }
            private set
            {
                location = value;
            }
        }

        public string Rank
        {
            get
            {
                return rank;
            }
            private set
            {
                rank = value;
            }
        }

        public float Offset
        {
            get
            {
                return offset;
            }
            private set
            {
                offset = value;
            }
        }

        public HOMode Mode
        {
            get
            {
                return mode;
            }
            private set
            {
                mode = value;
            }
        }

        public List<HOItem> Reward
        {
            get
            {
                return reward;
            }
            private set
            {
                reward = value;
            }
        }
        public Dictionary<string, string> Settings
        {
            get
            {
                return settings;
            }
            private set
            {
                settings = value;
            }
        }

        [SerializeField]
        private string[] activeItemFormula;
        [SerializeField]
        private string[] inactiveItemFormula;
        [SerializeField]
        private string[] staticItemFormula;

        public string[] ActiveItemFormula
        {
            get
            {
                return activeItemFormula;
            }
            private set
            {
                activeItemFormula = value;
            }
        }

        public string[] InactiveItemFormula
        {
            get
            {
                return inactiveItemFormula;
            }
            private set
            {
                inactiveItemFormula = value;
            }
        }

        public string[] StaticItemFormula
        {
            get
            {
                return staticItemFormula;
            }
            private set
            {
                staticItemFormula = value;
            }
        }
    }
}