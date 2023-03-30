using System.Collections.Generic;
using System.Linq;

namespace VectorTerrain.Scripts.Types
{
    public class Plot
    {
        public enum PlotType
        {
            Float,
            Signal,
            Mask,
            Weight
        }

        private readonly List<float> _xVals;
        public PlotType plotType;

        public List<float> YVals;

        // public Plot()
        // {
        //     _xVals = new();
        //     YVals = new();
        // }
        public Plot(PlotType plotType)
        {
            this.plotType = plotType;
            YVals = new List<float>();
            _xVals = new List<float>();
        }

        public Plot(PlotType plotType, List<float> yVals)
        {
            this.plotType = plotType;
            YVals = yVals;
            _xVals = new List<float>();
        }

        public List<float> Xvals
        {
            get
            {
                if (_xVals.Count == 0) initXvals();

                return _xVals;
            }
        }

        private void initXvals()
        {
            for (var i = 0; i < YVals.Count; i++) _xVals.Add(i - 1);
        }

        public void ScaleXVals(float start, float end)
        {
            if (_xVals.Count == 0) initXvals();
            var h = _xVals.Max();
            var l = _xVals.Min();


            for (var i = 0; i < _xVals.Count; i++) _xVals[i] = Hutl.Map(_xVals[i], l, h, start, end);
        }


        public void Clear()
        {
            _xVals.Clear();
            YVals.Clear();
        }
    }
}