using System;
using UnityEngine;

[Serializable]
public struct VfxSignalData
{
    public SpriteParticle vfx;
    public ExposedReference<Transform> targetTransform;
}

public class VfxSignalEmitter : BaseParamEmitter<VfxSignalData> { }

