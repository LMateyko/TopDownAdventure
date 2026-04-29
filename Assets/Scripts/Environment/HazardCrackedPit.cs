using UnityEngine;

public class HazardCrackedPit : HazardPit
{
    [SerializeField] Animator m_crackAnimator;

    bool CrackOpen => false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    protected override void Update()
    {
        // If the crack is open, do default pit behavior
        if(CrackOpen)
            base.Update();
    }
}
