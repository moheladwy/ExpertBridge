// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Entities.Profiles;

public static class ProfileEntityConstraints
{
    public const int JobTitleMaxLength = 256;
    public const int BioMaxLength = 5000;
    public const int RatingMinValue = 0;
    public const int RatingMaxValue = 5;
    public const int RatingCountMinValue = 0;
    public const int RatingCountMaxValue = int.MaxValue;
}
