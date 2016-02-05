// Copyright (c) The Perspex Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using System.Reflection;
using OmniXaml;
using OmniXaml.Typing;
using Perspex.Controls;

namespace Perspex.Markup.Xaml.Context
{
    public class PerspexXamlType : XamlType
    {
        public PerspexXamlType(Type type,
            ITypeRepository typeRepository,
            ITypeFactory typeFactory,
            ITypeFeatureProvider featureProvider) : base(type, typeRepository, typeFactory, featureProvider)
        {
        }

        public override OmniXaml.INameScope GetNamescope(object instance)
        {
            var result = instance as OmniXaml.INameScope;

            if (result == null)
            {
                var control = instance as Control;

                if (control != null)
                {
                    var perspexNs = (instance as Perspex.Controls.INameScope) ?? NameScope.GetNameScope(control);

                    if (perspexNs != null)
                    {
                        result = new NameScopeWrapper(perspexNs);
                    }
                }
            }

            return result;
        }

        protected override Member LookupMember(string name)
        {
            return new PerspexXamlMember(name, this, TypeRepository, FeatureProvider);
        }

        protected override AttachableMember LookupAttachableMember(string name)
        {
            // OmniXAML seems to require a getter and setter even though we don't use them.
            var getter = UnderlyingType.GetTypeInfo().GetDeclaredMethod("Get" + name);
            var setter = UnderlyingType.GetTypeInfo().GetDeclaredMethod("Set" + name);
            return new PerspexAttachableXamlMember(name, this, getter, setter, TypeRepository, FeatureProvider);
        }

        public override string ToString()
        {
            return "Perspex XAML Type " + base.ToString();
        }
    }
}