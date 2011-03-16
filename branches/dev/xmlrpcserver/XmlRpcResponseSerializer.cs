﻿/* 
XML-RPC.NET library
Copyright (c) 2001-2011, Charles Cook <charlescook@cookcomputing.com>

Permission is hereby granted, free of charge, to any person 
obtaining a copy of this software and associated documentation 
files (the "Software"), to deal in the Software without restriction, 
including without limitation the rights to use, copy, modify, merge, 
publish, distribute, sublicense, and/or sell copies of the Software, 
and to permit persons to whom the Software is furnished to do so, 
subject to the following conditions:

The above copyright notice and this permission notice shall be 
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
DEALINGS IN THE SOFTWARE.
*/

// TODO: overriding default mapping action in a struct should not affect nested structs

namespace CookComputing.XmlRpc
{
  using System;
  using System.Collections;
  using System.Globalization;
  using System.IO;
  using System.Reflection;
  using System.Text;
  using System.Text.RegularExpressions;
  using System.Threading;
  using System.Xml;
  using System.Collections.Generic;

  public class XmlRpcResponseSerializer : XmlRpcSerializer
  {
    public void SerializeResponse(Stream stm, XmlRpcResponse response)
    {
      Object ret = response.retVal;
      if (ret is XmlRpcFaultException)
      {
        SerializeFaultResponse(stm, (XmlRpcFaultException)ret);
        return;
      }
      XmlWriter xtw = XmlRpcXmlWriter.Create(stm, XmlEncoding, UseIndentation, Indentation); 
      xtw.WriteStartDocument();
      xtw.WriteStartElement("", "methodResponse", "");
      xtw.WriteStartElement("", "params", "");
      xtw.WriteStartElement("", "param", "");
      // TODO: use global action setting
      NullMappingAction mappingAction = NullMappingAction.Nil;
      try
      {
        Serialize(xtw, ret, mappingAction);
      }
      catch (XmlRpcUnsupportedTypeException ex)
      {
        throw new XmlRpcInvalidReturnType(string.Format(
          "Return value is of, or contains an instance of, type {0} which "
          + "cannot be mapped to an XML-RPC type", ex.UnsupportedType));
      }
      xtw.WriteEndElement();
      xtw.WriteEndElement();
      xtw.WriteEndElement();
      xtw.Flush();
    }
  }
}