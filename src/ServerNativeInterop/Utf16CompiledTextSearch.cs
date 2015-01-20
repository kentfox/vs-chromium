﻿// Copyright 2014 The Chromium Authors. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

using System;
using System.Collections.Generic;
using System.Linq;
using VsChromium.Core.Ipc.TypedMessages;
using VsChromium.Core.Utility;

namespace VsChromium.Server.NativeInterop {
  public abstract class Utf16CompiledTextSearch : ICompiledTextSearch {
    public abstract int PatternLength { get; }

    public virtual void Dispose() {
    }

    public IEnumerable<FilePositionSpan> SearchAll(TextFragment textFragment, IOperationProgressTracker progressTracker) {
      while (!textFragment.IsEmpty) {
        var searchHit = Search(textFragment);
        if (searchHit.IsNull)
          break;

        progressTracker.AddResults(1);
        if (progressTracker.ShouldEndProcessing)
          yield break;

        yield return new FilePositionSpan {
          // TODO(rpaquay): We are limited to 2GB for now.
          Position = (int)searchHit.CharacterOffset, 
          Length = (int)searchHit.CharacterCount
        };
        textFragment = textFragment.Suffix(searchHit.FragmentEnd);
      }
    }

    public FilePositionSpan? SearchOne(TextFragment textFragment, IOperationProgressTracker progressTracker) {
      return SearchAll(textFragment, progressTracker).FirstOrDefault();
    }

    public abstract TextFragment Search(TextFragment textFragment);
  }
}