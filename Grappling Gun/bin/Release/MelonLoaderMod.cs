using MelonLoader;
using StressLevelZero;
using StressLevelZero.Interaction;
using StressLevelZero.Combat;
using UnityEngine;
using BoneworksModdingToolkit;
using StressLevelZero.Props.Weapons;
using System.Windows;
using Boo.Lang.Compiler.TypeSystem;
using System.Linq;
using System.Collections.Generic;

namespace Grappling_Gun
{
    public static class BuildInfo
    {
        public const string Name = "Grappling_Gun"; // Name of the Mod.  (MUST BE SET)
        public const string Author = "Graic"; // Author of the Mod.  (Set as null if none)
        public const string Company = null; // Company that made the Mod.  (Set as null if none)
        public const string Version = "1.1.0"; // Version of the Mod.  (MUST BE SET)
        public const string DownloadLink = null; // Download Link for the Mod.  (Set as null if none)
    }

    public class Grappling_Gun_Controller
    {
        public bool isDead = false;
        GameObject go;
        LineRenderer lr;
        SpringJoint sj;
        bool connected = false;

        public Gun gun;
        Vector3 hitPoint;

        public void Init(Gun g, RaycastHit raycastHit) {
            go = g.gameObject;
            hitPoint = raycastHit.point;
            gun = go.GetComponent<Gun>();

            sj = go.AddComponent<SpringJoint>();
            sj.autoConfigureConnectedAnchor = false;
            sj.spring = ModPrefs.GetFloat(BuildInfo.Name, "strength");
            sj.damper = ModPrefs.GetFloat(BuildInfo.Name, "damper");
            sj.anchor = go.transform.InverseTransformPoint(gun.firePointTransform.position);

            Rigidbody rb = raycastHit.rigidbody;
            if (rb) {
                sj.connectedBody = rb;
                sj.connectedAnchor = rb.transform.InverseTransformPoint(hitPoint);
                connected = true;
            }
            else {
                sj.connectedAnchor = hitPoint;
            }
            
            lr = go.AddComponent<LineRenderer>();
            lr.material = new Material(Shader.Find("Unlit/Texture"));
            lr.material.color = Color.black;
            lr.startColor = Color.black;
            lr.endColor = Color.white;
            lr.startWidth = .05f;
            lr.endWidth = .05f;
        }

        public void Update() {
            if (gun.slideState == Gun.SlideStates.PULLING || (connected && sj.connectedBody==null)) {
                Object.Destroy(lr);
                Object.Destroy(sj);
                isDead = true;
            }
        }

        public void LateUpdate() {
            DrawGrapple();
        }

        void DrawGrapple() {
            lr.SetPosition(0, gun.firePointTransform.position);
            lr.SetPosition(1, connected ? sj.connectedBody.transform.TransformPoint(sj.connectedAnchor) : sj.connectedAnchor);
        }
    }

    public class Grappling_Gun : MelonMod
    {
        bool isEnabled = false;

        List<Grappling_Gun_Controller> ggcs = new List<Grappling_Gun_Controller>();

        public string gunname;

        Gun[] guns;

        public override void OnApplicationStart() {
            MelonModLogger.Log("Press G to toggle Grappling Gun keybinds");
            RegisterPrefs();
            LoadPrefs();
        }

        public override void OnUpdate() {
            if (!Player.FindPlayer()) return; // Don't run in loading screens
            if (Input.GetKeyDown(KeyCode.G)) { // Toggle mod
                isEnabled = !isEnabled;
                MelonModLogger.Log("Grappling Gun is now " + (isEnabled ? "enabled" : "disabled"));
                return; // idk this stops it from crashing
            }
            if (!isEnabled) return;

            // Check for shot
            Projectile[] projectileArray = Object.FindObjectsOfType<Projectile>();
            if (projectileArray.Length > 0) { // Only do this if theres bullets for preformance reasons
                 guns = Object.FindObjectsOfType<Gun>();
            }
            for (int index = 0; index < projectileArray.Length; ++index) { // Loop over all bullets
                RaycastHit[] raycastHitArray = Physics.RaycastAll(projectileArray[index].transform.position, projectileArray[index]._direction, 10f);
                if (raycastHitArray.Length > 0) {
                    RaycastHit raycastHit = raycastHitArray[0]; // Find closest hit
                    foreach (RaycastHit rH in raycastHitArray) {
                        if (rH.distance < raycastHit.distance)
                            raycastHit = rH;
                    }

                    foreach (Gun gun in guns) {
                        if (gun.name == projectileArray[index].weaponName && gun.name.ToLower().Contains(gunname)) {
                            bool pass = true;
                            foreach (Grappling_Gun_Controller ggc in ggcs) { // Check if theres already a grapple
                                if (ggc.gun == gun) {
                                    pass = false;
                                    break;
                                }
                            }
                            if (pass) { // Add a grapple
                                Grappling_Gun_Controller newggc = new Grappling_Gun_Controller();
                                newggc.Init(gun, raycastHit);
                                ggcs.Add(newggc);
                            }
                            break;
                        }
                    }
                }
            }

            for (int i = ggcs.Count - 1; i >= 0; i--) { // update and remove grapples
                ggcs[i].Update();
                if (ggcs[i].isDead)
                    ggcs.Remove(ggcs[i]);
            }
        }

        public override void OnLateUpdate() { // Update line renders
            if (!Player.FindPlayer()) return;
            foreach (Grappling_Gun_Controller ggc in ggcs) {
                ggc.LateUpdate();
            }
        }

        void RegisterPrefs() { // settings
            ModPrefs.RegisterCategory(BuildInfo.Name, "Grappling Gun");
            ModPrefs.RegisterPrefString(BuildInfo.Name, "gunname", "eder");
            ModPrefs.RegisterPrefFloat(BuildInfo.Name, "strength", 100);
            ModPrefs.RegisterPrefFloat(BuildInfo.Name, "damper", 10);
        }

        void LoadPrefs() { // more settings
            gunname = ModPrefs.GetString(BuildInfo.Name, "gunname").ToLower();
        }
    }
}
