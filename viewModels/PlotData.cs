using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lside_Mixture.viewModels
{
    public class PlotData<T>
    {
        public PlotData(string title, T[] data)
        {
            this.Title = title;
            this.Data = data;
        }

        public string Title { get; set; }

        public T[] Data { get; set; }
    }
}
