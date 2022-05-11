using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TestProject1.Editor
{
    [CustomEditor(typeof(MeleeWeaponData))]
    public class MeleeWeaponCustomEditorUI : UnityEditor.Editor
    {
        private VisualElement root;
        private VisualTreeAsset visualTree;

#if UNITY_EDITOR
        private void OnEnable()
        {
            root = new VisualElement();
            visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/TestProject1/UI/UIEditor.uxml");
        }
#endif
        public override VisualElement CreateInspectorGUI()
        {
            root.Clear();
            visualTree.CloneTree(root);

            // capturar campos a vincular
            var weaponNameField = root.Q<TextField>("wNameText");
            var weaponImageField = root.Q<ObjectField>("wSprite");
            var weaponRarityField = root.Q<SliderInt>("wRarity");
            var weaponDamageField = root.Q<SliderInt>("wDamage");
            var weaponDurabilityField = root.Q<SliderInt>("wDurability");

            var headerLabel = root.Q<Label>("WeaponNameText");
            var meleeWeaponImage = root.Q<VisualElement>("MeleeWeaponImage");
            // vincular campos con propiedades
            weaponNameField.BindProperty(serializedObject.FindProperty("weaponName"));
            weaponImageField.BindProperty(serializedObject.FindProperty("weaponImage"));
            weaponRarityField.BindProperty(serializedObject.FindProperty("weaponRarity"));
            weaponDamageField.BindProperty(serializedObject.FindProperty("weaponDamage"));
            weaponDurabilityField.BindProperty(serializedObject.FindProperty("weaponDurability"));

            // registrar callbacks events al cambiar los valores de los campos
            weaponNameField.RegisterValueChangedCallback(ctx => ChangeHeaderTittle(headerLabel, weaponNameField));
            weaponImageField.RegisterValueChangedCallback(ctx => ChangeImage(meleeWeaponImage, weaponImageField));
            weaponRarityField.RegisterValueChangedCallback(ctx => ChangeImageBorderColor(meleeWeaponImage,weaponRarityField));
            return root;
        }

        private void ChangeImageBorderColor(VisualElement meleeWeaponImage, SliderInt weaponRarityField)
        {
            Color newBorderColor;

            newBorderColor = weaponRarityField.value switch
            {
                1=>Color.white,
                2=>Color.cyan,
                3=>Color.magenta,
                4=>Color.yellow,
                _=>Color.black,
            };

            meleeWeaponImage.style.borderTopColor = newBorderColor;
            meleeWeaponImage.style.borderBottomColor = newBorderColor;
            meleeWeaponImage.style.borderLeftColor = newBorderColor;
            meleeWeaponImage.style.borderRightColor = newBorderColor;
        }

        private void ChangeHeaderTittle(Label myLabel, TextField myTextField) 
        {
            myLabel.text = $"Melee Weapon: {myTextField.text}";
        }

        // la imagen es el background image de un VisualElement
        private void ChangeImage(VisualElement myVisualElement, ObjectField mySprite) 
        {
            myVisualElement.style.backgroundImage = Background.FromSprite(mySprite.value as Sprite);
        }
    }
}
