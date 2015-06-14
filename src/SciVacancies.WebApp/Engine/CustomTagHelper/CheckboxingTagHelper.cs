﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.AspNet.Razor.Runtime.TagHelpers;

namespace SciVacancies.WebApp
{
    [TargetElement("checkboxing", Attributes = "items, values, property")]
    public class CheckboxingTagHelper : TagHelper
    {
        public IEnumerable<SelectListItem> Items { get; set; }
        public IEnumerable<string> Values { get; set; }
        public string Property { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            /*
                <li>
                    <span class="checkbox">
                        <input type="checkbox" />
                    </span><label>Владивосток (12)</label>
                </li>
            */
            var i = 1;
            foreach (var item in Items)
            {
                var input = new TagBuilder("input");
                var span = new TagBuilder("span");

                input.Attributes.Add("type", "checkbox");
                input.Attributes.Add("name", Property);
                input.Attributes.Add("id", Property + i);
                input.Attributes.Add("value", item.Value);

                if (Values.Contains(item.Value))
                {
                    input.Attributes.Add("checked", string.Empty);
                    span.AddCssClass("checked");
                }

                span.AddCssClass("checkbox");
                span.InnerHtml = input.ToString(TagRenderMode.SelfClosing);

                var label = new TagBuilder("label");
                label.Attributes.Add("for", input.Attributes["id"]);
                label.SetInnerText(item.Text);

                var li = new TagBuilder("li")
                {
                    InnerHtml = span.ToString() + label
                };

                output.Content.Append(li.ToString());
                i++;
            }

            //output.Attributes.Add("itemscope", null);
            //output.Attributes.Add("itemtype", "http://schema.org/Organization");
            //var name = new TagBuilder("span");
            //name.MergeAttribute("itemprop", "name");

            //var span = new TagBuilder("span");
            //span.MergeAttribute("itemprop", "streetAddress");
            //span.SetInnerText(Organisation.StreetAddress);
            //address.InnerHtml = span.ToString() + br;

            //span.MergeAttribute("itemprop", "postalCode");
            //span.SetInnerText($" {Organisation.PostalCode}");

            //address.InnerHtml += span.ToString();



        }
    }

}
