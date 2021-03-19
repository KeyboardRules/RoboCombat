using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;
    [SerializeField] Menu[] menus;
    private void Awake()
    {
        Instance = this;
    }
    public void OpenMenu(string menuName)
    {
        foreach (Menu m in menus)
        {
            if (m.menuName.Equals(menuName))
            {
                m.Open();
            }
            else if (m.isOpen)
            {
                m.Close();
            }
        }
    }
    public void OpenMenu(Menu menu)
    {
        foreach (Menu m in menus)
        {
            if (m.isOpen)
            {
                CloseMenu(m);
            }
        }
        menu.Open();
    }
    public void ResponsiveMenu(string menuName)
    {
        foreach (Menu m in menus)
        {
            if (m.menuName.Equals(menuName) && m.isOpen)
            {
                m.Close();
            }
            else if(m.menuName.Equals(menuName) && !m.isOpen)
            {
                m.Open();
            }
        }
    }
    public void CloseMenu(Menu menu)
    {
        menu.Close();
    }
}
