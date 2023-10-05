﻿using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace Nans.Hoi4.Modding.Tool.XAML
{
    public class LocalizeExtension : MarkupExtension
    {
        public string Key { get; set; }
        public Binding KeySource { get; set; }
        public LocalizeExtension() { }
        public LocalizeExtension(string key)
        {
            Key = key;
        }
        public LocalizeExtension(Binding keySource)
        {
            KeySource = keySource;
        }
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            IProvideValueTarget providerValueTarget = serviceProvider as IProvideValueTarget;
            MultiBinding multiBinding = new MultiBinding()
            {
                Converter = new LocalizationConverter(Key),
                NotifyOnSourceUpdated = true
            };
            if (KeySource != null)
            {
                multiBinding.Bindings.Add(KeySource);
            }

            return multiBinding.ProvideValue(serviceProvider);
        }
    }
}
