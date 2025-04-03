namespace Raytracer.Core;

using Raytracer.Core.Math;

public class Transform
{
    private Vector3 _position;
    public Vector3 position
    {
        get => _position;
        set => UpdatePosition(value);
    }

    private Vector3 _localPosition;
    public Vector3 localPosition
    {
        get => _localPosition;
        set => this.UpdateLocalPosition(value);
    }

    private Quaternion _rotation;
    public Quaternion rotation
    {
        get => _rotation;
        set => UpdateRotation(value);
    }

    private Quaternion _localRotation;
    public Quaternion localRotation
    {
        get => _localRotation;
        set => this.UpdateLocalRotation(value);
    }

    private Transform? _parent;
    public Transform? parent
    {
        get => _parent;
        set
        {
            _parent = value;
            _parent?.AddChild(this);
        }
    }

    private List<Transform> children = new List<Transform>();

    public delegate void PositionChangedEvent();
    public event PositionChangedEvent? PositionChanged;

    public delegate void RotationChangedEvent();
    public event RotationChangedEvent? RotationChanged;

    public Transform(Vector3 position, Quaternion rotation)
    {
        this.position = position;
        this.rotation = rotation;
    }

    private void UpdateLocalPosition(Vector3 pos)
    {
        _position = TransformToWorldPosition(pos);
        _localPosition = pos;

        PositionChanged?.Invoke();

        // Propagate position change to children
        foreach (var child in children)
        {
            child.UpdatePosition(child.TransformToLocalPosition(_position));
        }
    }

    private void UpdatePosition(Vector3 pos)
    {
        _localPosition = TransformToLocalPosition(pos);
        _position = pos;

        PositionChanged?.Invoke();

        // Propagate position change to children
        foreach (var child in children)
        {
            child.UpdatePosition(child.TransformToLocalPosition(_position));
        }
    }

    private void UpdateLocalRotation(Quaternion rot)
    {
        _rotation = parent != null ? parent.rotation * rot : rot;
        _localRotation = rot;

        RotationChanged?.Invoke();

        // Propagate rotation change to children
        foreach (var child in children)
        { 
            child.UpdateRotation(child.TransformToLocalRotation(_rotation));
        }
    }

    private void UpdateRotation(Quaternion rot)
    {
        _localRotation = parent != null ? parent.rotation.Inverse * rot : rot;
        _rotation = rot;

        RotationChanged?.Invoke();

        // Propagate rotation change to children
        foreach (var child in children)
        {
            child.UpdateRotation(child.TransformToLocalRotation(_rotation));
        }
    }

    public void Translate(float x, float y, float z)
    {
        this.position = this.position + new Vector3(x, y, z);
    }

    public void MoveTo(float x, float y, float z)
    {
        this.position = new Vector3(x, y, z);
    }

    public void MoveTo(Vector3 pos)
    {
        this.position = pos;
    }

    public List<Transform> GetChildren()
    {
        return children;
    }

    public void AddChild(Transform child)
    {
        this.children.Add(child);

        if (child.parent != this)
        {
            child.parent = this;
        }
    }

    public Vector3 TransformToWorldPosition(Vector3 localPos)
    {
        Vector3 worldPosition = rotation * localPos + position;

        if (parent != null)
        {
            worldPosition = parent.TransformToWorldPosition(worldPosition);
        }

        return worldPosition;
    }

    public Vector3 TransformToLocalPosition(Vector3 pos)
    {
        if (parent != null)
        {
            pos = parent.TransformToLocalPosition(pos);
        }

        return rotation.Inverse * (pos - position);
    }

    public Quaternion TransformToWorldRotation(Quaternion localRot)
    {
        return rotation * localRot;
    }

    public Quaternion TransformToLocalRotation(Quaternion worldRot)
    {
        return parent != null ? parent.rotation.Inverse * worldRot : worldRot;
    }
}