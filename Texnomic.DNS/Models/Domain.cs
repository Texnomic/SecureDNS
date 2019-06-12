using BinarySerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Texnomic.DNS.Enums;

namespace Texnomic.DNS.Models
{
    public class Domain
    {
        [FieldOrder(17)]
        [SerializeUntil((byte)0)]
        public Label[] Labels { get; set; }

        public override string ToString()
        {
            return string.Join('.', Labels.Select(Label => Label.Text));
        }

        public static Domain FromString(string Text)
        {
            var TextLables = Text.Split('.');

            var Domain = new Domain
            {
                Labels = new Label[TextLables.Length]
            };

            for (int i = 0; i < TextLables.Length; i++)
            {
                Domain.Labels[i] = new Label
                {
                    LabelType = LabelType.Normal,
                    Count = (ushort)TextLables[i].Length,
                    Text = TextLables[i]
                };
            }

            return Domain;
        }
    }
}
