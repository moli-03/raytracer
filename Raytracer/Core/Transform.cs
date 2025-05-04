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

    private Vector3 _scale = Vector3.UnitX + Vector3.UnitY + Vector3.UnitZ; // Default 1,1,1
    public Vector3 Scale 
    {
        get => _scale;
        set 
        {
            _scale = value;
            ScaleChanged?.Invoke();
        }
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

    public delegate void ScaleChangedEvent();
    public event ScaleChangedEvent? ScaleChanged;

    public Transform(Vector3 position, Quaternion rotation)
    {
        this.position = position;
        this.rotation = rotation;
        this.Scale = Vector3.UnitX + Vector3.UnitY + Vector3.UnitZ; // (1,1,1)
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
        // Apply scale to the local position
        Vector3 scaledPos = new Vector3(
            localPos.X * _scale.X,
            localPos.Y * _scale.Y,
            localPos.Z * _scale.Z
        );
        
        Vector3 worldPosition = rotation * scaledPos + position;

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

        Vector3 localPos = rotation.Inverse * (pos - position);
        
        // Remove scale from local position
        return new Vector3(
            _scale.X != 0 ? localPos.X / _scale.X : localPos.X,
            _scale.Y != 0 ? localPos.Y / _scale.Y : localPos.Y,
            _scale.Z != 0 ? localPos.Z / _scale.Z : localPos.Z
        );
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