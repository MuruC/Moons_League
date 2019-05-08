using UnityEngine;
using System.Collections.Generic;

namespace FoW
{
    class FOWUnit
    {
        public Vector3 destination;
        public FogOfWarUnit unit;
        public Transform transform { get; private set; }
        public Vector3 position { get { return transform.position; } set { transform.position = value; } }

        public FOWUnit(FogOfWarUnit u)
        {
            unit = u;
            transform = unit.transform;
            destination = unit.transform.position;
        }
    }
    
    public class FogOfWar3DTest : MonoBehaviour
    {
        public int team = 0;
        public Camera mainCamera;
        public float unitMoveSpeed = 3.0f;
        public float cameraSpeed = 20.0f;
        public Transform highlight;
        public float unfogSize = 2;
        
        FogOfWarTeam _team { get { return FogOfWarTeam.GetTeam(team); } }

        List<FOWUnit> _units = new List<FOWUnit>();

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
                return;
            }

            // select unit
            if (Input.GetKeyDown(KeyCode.Mouse0) && Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
            {
                FogOfWarUnit unit = hit.collider.GetComponent<FogOfWarUnit>();
                if (unit != null && unit.team == team)
                {
                    int index = _units.FindIndex(((u) => u.unit == unit));
                    if (index != -1)
                    {
                        _units.Add(_units[index]);
                        _units.RemoveAt(index);
                    }
                    else
                        _units.Add(new FOWUnit(unit));
                }
            }

            // move unit
            if (_units.Count > 0 && Input.GetKeyDown(KeyCode.Mouse1))
            {
                RaycastHit[] hits = Physics.RaycastAll(mainCamera.ScreenPointToRay(Input.mousePosition));
                if (hits.Length > 0)
                {
                    Vector3 p = hits[hits.Length - 1].point;
                    p.y = 1.0f;
                    _units[_units.Count - 1].destination = p;
                }
            }

            // clear fogged area
            if (Input.GetKeyDown(KeyCode.Mouse2) && Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hit))
                _team.SetFog(new Bounds(hit.point, Vector3.one * unfogSize), 0);

            // update units
            float moveamount = unitMoveSpeed * Time.deltaTime;
            for (int i = 0; i < _units.Count; ++i)
            {
                FOWUnit u = _units[i];
                Vector3 direction = u.destination - u.position;
                direction.y = 0.0f;
                if (direction.sqrMagnitude < moveamount * moveamount)
                    u.position = new Vector3(u.destination.x, u.position.y, u.destination.z);
                else
                {
                    u.position += direction.normalized * moveamount;
                    u.transform.rotation = Quaternion.Slerp(u.transform.rotation, Quaternion.LookRotation(direction, Vector3.up), moveamount);
                }
            }

            // update highlight
            if (_units.Count > 0)
            {
                highlight.position = new Vector3(_units[_units.Count - 1].position.x, 0.1f, _units[_units.Count - 1].position.z);
                highlight.gameObject.SetActive(true);
            }

            // update camera
            Transform camtransform = mainCamera.transform;
            camtransform.position += new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * (Time.deltaTime * cameraSpeed);
            
            if (Input.touchCount == 1)
            {
                Vector2 delta = Input.GetTouch(0).deltaPosition;
                camtransform.position += new Vector3(-delta.x, 0, -delta.y);
            }

            // update camera zooming
            float zoomchange = Input.GetAxis("Mouse ScrollWheel");
            camtransform.position = new Vector3(camtransform.position.x, Mathf.Clamp(camtransform.position.y - zoomchange * 10, 25, 50), camtransform.position.z);
        }
    }
}