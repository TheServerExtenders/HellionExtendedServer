// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Network.ResponseResult
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

namespace ZeroGravity.Network
{
  public enum ResponseResult : short
  {
    OwnershipIssue = -5,
    WrongPassword = -4,
    AlreadyLoggedInError = -3,
    ClientVersionError = -2,
    Error = -1,
    Success = 1,
  }
}
