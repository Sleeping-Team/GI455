using System;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

[RequireComponent(typeof(Animator))]
public class CustomerAnimated : NetworkBehaviour
{
    private Customer m_Customer;
    private Animator m_Animator;
    
    private NetworkAnimator m_NetworkAnimator;
    
    private bool m_IsServerAuthoritative = true;
    
    private bool m_Move;
    private bool m_Sit;
    private bool m_Idle;
    
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
        m_Customer = transform.parent.GetComponent<Customer>();
        m_Animator = GetComponent<Animator>();
        m_Move = false;
        m_Sit = false;
        m_Idle = true;
    }    
    private bool IsServerAuthority()
    {
        if (m_IsServerAuthoritative)
        {
            return true;
        }

        return false;
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void ToggleWalkingAnimationServerRpc(bool move)
    {
        m_Move = move;
        m_Animator.SetTrigger("IsWalk");
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void ToggleIdleAnimationServerRpc(bool idle)
    {
        m_Idle = idle;
        m_Animator.SetTrigger("IsIdle");
    }

    [ServerRpc(RequireOwnership = false)]
    private void ToggleSitAnimationServerRpc(bool sit)
    {
        m_Sit = sit;
        m_Animator.SetTrigger("IsSit");
    }
    
    private void PlayWalkingAnimation(bool isMoving)
    {
        m_Move = isMoving;
        if (isMoving == false)
        {
            return;
        }
        if (m_IsServerAuthoritative)
        {
            ToggleWalkingAnimationServerRpc(m_Move);
        }
        else
        {
            m_Animator.SetTrigger("IsWalk");
        }
    }
    
    private void PlayIdleAnimation(bool isIdle)
    {
        m_Idle = isIdle;
        if (isIdle == false)
        {
            return;
        }
        if (m_IsServerAuthoritative)
        {
            ToggleIdleAnimationServerRpc(m_Idle);
        }
        else
        {
            m_Animator.SetTrigger("IsIdle");
        }
    }
    
    private void PlaySitAnimation(bool isSit)
    {
        m_Sit = isSit;
        if (isSit == false)
        {
            return;
        }
        if (m_IsServerAuthoritative)
        {
            ToggleIdleAnimationServerRpc(m_Sit);
        }
        else
        {
            m_Animator.SetTrigger("IsSit");
        }
    }
    
    private void LateUpdate()
    {
        if (m_Customer.IsWalk)
        {
            //walk
            PlayWalkingAnimation(true);
            PlayIdleAnimation(false);
            PlaySitAnimation(false);
            Debug.Log("Customer walk");
        }
        else if(m_Customer.IsSit)
        {
            //Sit
            PlayWalkingAnimation(false);
            PlayIdleAnimation(false);
            PlaySitAnimation(true);
            Debug.Log("Customer Sit");
        }
        else
        {
            //Idle
            PlayWalkingAnimation(false);
            PlayIdleAnimation(true);
            PlaySitAnimation(false);
            Debug.Log("Customer Idle");
        }
    }
}
