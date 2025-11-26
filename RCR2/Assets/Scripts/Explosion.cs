using Photon.Pun;
using UnityEngine;

public class Explosion : MonoBehaviourPun
{
    public float Radius;
    public float Force;
    public bool Active;
    public GameObject ExplossionEffect;

    private bool hasExploded = false;

    private void Update()
    {
        if (Active && !hasExploded && photonView.IsMine)
        {
            photonView.RPC("RPC_Explode", RpcTarget.All);
        }
    }

    [PunRPC]
    public void RPC_Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        Debug.Log($"Explosion synchronized for {photonView.Owner.NickName}");

        // Физика взрыва - только на мастер-клиенте для избежания десинхронизации
        if (PhotonNetwork.IsMasterClient)
        {
            Collider[] overlappedColliders = Physics.OverlapSphere(transform.position, Radius);

            for (int i = 0; i < overlappedColliders.Length; i++)
            {
                Rigidbody rigidbody = overlappedColliders[i].attachedRigidbody;
                if (rigidbody != null)
                {
                    rigidbody.AddExplosionForce(Force, transform.position, Radius);

                    // Наносим урон игрокам
                    PlayerInfo playerInfo = rigidbody.GetComponent<PlayerInfo>();
                    if (playerInfo != null && playerInfo.photonView.IsMine)
                    {
                        float distance = Vector3.Distance(transform.position, rigidbody.position);
                        int damage = Mathf.RoundToInt((1 - distance / Radius) * 50f);
                        playerInfo.TakeDamage(damage);
                    }
                }
            }
        }

        // Визуальные эффекты - на всех клиентах
        var explosion = Instantiate(ExplossionEffect, transform.position, Quaternion.identity);
        Destroy(explosion, 1f);

        Active = false;

        // Уничтожаем объект через сеть
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    public void Explode()
    {
        if (photonView.IsMine)
        {
            Active = true;
        }
    }
}