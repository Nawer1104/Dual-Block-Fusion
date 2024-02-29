using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    public int type;

    private Animator anim;

    public GameObject vfxDestroy;

    public bool isSelected;

    public Transform[] points;

    private bool isSomethingInFront;

    public LayerMask layer;

    public SpriteRenderer sprite;

    private string hexColorDim = "#716A6A";

    private string hexColorWhite = "#FFFFFF";

    private void Awake()
    {
        anim = GetComponent<Animator>();

        isSelected = false;
    }

    private void Update()
    {
        if (isSelected) return;

        isSomethingInFront = CheckFront();

        SetGameObjectBeha();
    }

    private void OnMouseDown()
    {
        if (isSomethingInFront) return;

        if (isSelected || GameManager.Instance.levels[GameManager.Instance.GetCurrentIndex()].selectedObjects.Count >= GameManager.Instance.levels[GameManager.Instance.GetCurrentIndex()].collectionTransform.Count)
        {
            GameManager.Instance.levels[GameManager.Instance.GetCurrentIndex()].RemoveMatchedObjectsWhenFull(this);
            return;
        }

        isSelected = true;

        GetComponent<BoxCollider2D>().enabled = false;

        GetComponent<Rigidbody2D>().simulated = false;

        GameManager.Instance.levels[GameManager.Instance.GetCurrentIndex()].SelectObject(gameObject);
    }

    public void PlayAnimSelected()
    {
        anim.SetTrigger("Selected");
    }

    public void PlayAnimDestroy()
    {
        anim.SetTrigger("Destroy");
    }

    public void PlayDestroyVfx()
    {
        GameObject vfx = Instantiate(vfxDestroy, transform.position, Quaternion.identity) as GameObject;
        Destroy(vfx, 1f);
        GameManager.Instance.levels[GameManager.Instance.GetCurrentIndex()].gameobjects.Remove(gameObject);
        GameManager.Instance.CheckLevelUp();
    }

    private void OnDrawGizmos()
    {
        foreach(Transform point in points)
        {
        Gizmos.DrawLine(point.position, new Vector3(point.position.x, point.position.y, point.position.z * -1));
        }
    }

    private bool CheckFront()
    {
        foreach (Transform point in points)
        {
            RaycastHit2D hit = Physics2D.Raycast(point.position, new Vector3(point.position.x, point.position.y, point.position.z * -1), Mathf.Infinity, layer);

            if (hit.collider != null && hit.collider.name != gameObject.name)
            {
                return true;
            }
        }

        return false;
    }

    private void SetGameObjectBeha()
    {
        if (isSomethingInFront)
        {
            Color color = HexToColor(hexColorDim);
            sprite.color = color;
            //transform.position = new Vector3(transform.position.x, transform.position.y, 1f);
        }
        else
        {
            Color color = HexToColor(hexColorWhite);
            sprite.color = color;
            //transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
        }
    }

    // Function to convert hexadecimal color string to Color
    Color HexToColor(string hex)
    {
        // Remove the '#' character if present
        if (hex.StartsWith("#"))
        {
            hex = hex.Remove(0, 1);
        }

        // Parse hexadecimal value to Color components
        Color color = new Color();
        if (ColorUtility.TryParseHtmlString("#" + hex, out color))
        {
            return color;
        }
        else
        {
            Debug.LogWarning("Invalid hexadecimal color: " + hex);
            return Color.white; // Return white color as fallback
        }
    }
}
