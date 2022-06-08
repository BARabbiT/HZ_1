using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class FlyingCamera : MonoBehaviour
    {

        public Camera cam;

        private void Update()
        {

            transform.position = Vector3.MoveTowards(transform.position, transform.position + (cam.transform.forward * Input.GetAxis("Vertical")) + (transform.right * Input.GetAxis("Horizontal")), Time.deltaTime * 10f);
            transform.Rotate(new Vector3(-Input.GetAxis("Mouse Y"), 0, 0));
            cam.transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X"), 0));

            if (Input.GetMouseButtonDown(0))
            {

                Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1f));
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {

                    if (hit.transform.tag == "Terrain")
                    {
                        MeshFilter mesh = hit.transform.GetComponent<MeshFilter>();
                        var index = Array.IndexOf(mesh.mesh.vertices, hit.point);
                        var oldV = mesh.mesh.vertices[index];
                        oldV.y += 1f;
                        mesh.mesh.vertices[index] = oldV;

                    }

                }

            }

            if (Input.GetMouseButtonDown(1))
            {

                Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1f));
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {

                    if (hit.transform.tag == "Terrain")
                    {
                        MeshFilter mesh = hit.transform.GetComponent<MeshFilter>();
                        var index = Array.IndexOf(mesh.mesh.vertices, hit.point);
                        var oldV = mesh.mesh.vertices[index];
                        oldV.y -= 1f;
                        mesh.mesh.vertices[index] = oldV;

                    }

                }

            }

        }
    }
}
