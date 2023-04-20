using System;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

[RequireComponent(typeof(Animator))]
public class AnimatedPlayerController : NetworkBehaviour
{
    private PlayerMovement m_PlayerMovement;
    private Animator m_Animator;
    private bool m_Move;
    private NetworkAnimator m_NetworkAnimator;
    private bool m_IsServerAuthoritative = true;
    
    private void DetermineNetworkAnimatorComponentType()
    {
        m_NetworkAnimator = GetComponent<NetworkAnimator>();
        if (m_NetworkAnimator != null)
        {
            m_IsServerAuthoritative = m_NetworkAnimator.GetType() != typeof(OwnerNetworkAnimator);
        }
        else
        {
            throw new Exception(
                $"{nameof(AnimatedPlayerController)} requires that is it paired with either a {nameof(NetworkAnimator)} or {nameof(OwnerNetworkAnimator)}. Neither of the two components were found!");
        }
    }

    public override void OnNetworkSpawn()
    {
        DetermineNetworkAnimatorComponentType();
        m_PlayerMovement = transform.parent.GetComponent<PlayerMovement>();
        m_Animator = GetComponent<Animator>();
        m_Move = m_Animator.GetBool("Walking");
    }

    private bool HasAuthority()
    {
        if (IsOwnerAuthority() || IsServerAuthority())
        {
            return true;
        }

        return false;
    }
    
    private bool IsServerAuthority()
    {
        if (IsServer && m_IsServerAuthoritative)
        {
            return true;
        }

        return false;
    }

    private bool IsOwnerAuthority()
    {
        if (IsOwner && !m_IsServerAuthoritative)
        {
            return true;
        }

        return false;
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void ToggleWalkingAnimationServerRpc(bool move)
    {
        m_Move = move;
        m_Animator.SetBool("Walking", m_Move);
    }

    private void PlayWalkingAnimation(bool isMoving)
    {
        m_Move = isMoving;
        if (m_IsServerAuthoritative)
        {
            if (!IsServer && IsOwner)
            {
                ToggleWalkingAnimationServerRpc(m_Move);
            }
            else if (IsServer && IsOwner)
            {
                m_Animator.SetBool("Walking", m_Move);
            }
        }
        else if (IsOwner)
        {
            m_Animator.SetBool("Walking", m_Move);
        }
    }

    private void LateUpdate()
    {
        if (!IsServer || !IsOwner)
        {
            return;
        }

        if (m_PlayerMovement.IsMoving)
        {
            PlayWalkingAnimation(true);
        }
        else
        {
            PlayWalkingAnimation(false);
        }
    }
}
