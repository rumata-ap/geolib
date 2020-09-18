using Unitful
uType = Union{Float64,Int64,Quantity,Nothing}

"""
### Точка на плоскости с изменяемыми значениями координат
поле `attr` : словарь для записи дополнительных атрибутов точки.
"""
mutable struct point2d!
    x::uType
    y::uType
    attr::Dict{String,uType}

    point2d!() = new()
    point2d!(x, y) = new(x, y)
    point2d!(x, y, attr) = new(x, y, attr)
end

"""
### Точка на плоскости с неизменяемыми значениями координат
поле `attr` : словарь для записи дополнительных атрибутов точки.
"""
struct point2d
    x::Float64
    y::Float64
    attr::Dict{String,Float64}

    point2d(x, y, attr=Dict(""=>0)) = new(x, y, attr)
    point2d(x, y, attr) = new(x, y, attr)
end

"""
### Точка в пространстве с изменяемыми значениями координат
поле `attr` : словарь для записи дополнительных атрибутов точки.
"""
mutable struct point3d!
    x::Float64
    y::Float64
    z::Float64
    attr::Dict{String,Float64}

    point3d!() = new()
    point3d!(x,y,z) = new(x,y,z)
    point3d!(x,y,z,attr) = new(x,y,z,attr)
end

"""
### Точка в пространстве с неизменяемыми значениями координат
поле `attr` : словарь для записи дополнительных атрибутов точки.
"""
struct point3d
    x::Float64
    y::Float64
    z::Float64
    attr::Dict{String,Float64}

    point3d(x, y, z, attr=Dict(""=>0)) = new(x, y, z, attr)
    point3d(x, y, z, attr) = new(x, y, z, attr)
end

"""
### Вектор в пространстве с неизменяемыми значениями координат
поле `arr` : представление вектора в виде массива.
"""
struct vector3d
    vx::Float64
    vy::Float64
    vz::Float64
    arr::Vector{Float64}

    vector3d(vx, vy, vz, arr=[vx,vy,vz]) = new(vx, vy, vz, arr)

    function vector3d(pt1::point3d,pt2::point3d)
        vx = pt2.x - pt1.x
        vy = pt2.y - pt1.y
        vz = pt2.z - pt1.z
        arr = [vx, vy, vz]
        new(vx, vy, vz, arr)
    end

    function vector3d(pt1::point3d!,pt2::point3d!)
        vx = pt2.x - pt1.x
        vy = pt2.y - pt1.y
        vz = pt2.z - pt1.z
        arr = [vx, vy, vz]
        new(vx, vy, vz, arr)
    end
end

"""
### Вектор на плоскости с неизменяемыми значениями координат
поле `arr` : представление вектора в виде массива.
"""
struct vector2d
    vx::Float64
    vy::Float64
    arr::Vector{Float64}

    vector2d(vx, vy, arr=[vx,vy]) = new(vx, vy, arr)

    function vector2d(pt1::point2d,pt2::point2d)
        vx = pt2.x - pt1.x
        vy = pt2.y - pt1.y
        arr = [vx, vy]
        new(vx, vy, arr)
    end

    function vector2d(pt1::point2d!,pt2::point2d!)
        vx = pt2.x - pt1.x
        vy = pt2.y - pt1.y
        arr = [vx, vy]
        new(vx, vy, arr)
    end
end

import Base.*
*(v1::vector3d, v2::vector3d) = vector3d(v1.vy * v2.vz - v1.vz * v2.vy,
                                        v1.vz * v2.vx - v1.vx * v2.vz,
                                        v1.vx * v2.vy - v1.vy * v2.vx)

"""
### Плоскость, определяемая 3-мя точками
поле `arr` : массив [A, B, C, D] коэффициентов уравнения плоскости.
"""
struct plane
    A::Float64
    B::Float64
    C::Float64
    D::Float64
    arr::Vector{Float64}

    function plane(pt1::point3d, pt2::point3d, pt3::point3d)
        V1 = vector3d(pt1, pt2)
        V2 = vector3d(pt1, pt3)
        A = V1.vy * V2.vz - V1.vz * V2.vy #A
        B = V1.vz * V2.vx - V1.vx * V2.vz #B
        C = V1.vx * V2.vy - V1.vy * V2.vx #C
        D = -A * pt1.x - B * pt1.y - C * pt1.z #D
        arr = [A, B, C, D]
        new(A, B, C, D, arr)
    end

    function plane(pt1::point3d!, pt2::point3d!, pt3::point3d!)
        V1 = vector3d(pt1, pt2)
        V2 = vector3d(pt1, pt3)
        A = V1.vy * V2.vz - V1.vz * V2.vy #A
        B = V1.vz * V2.vx - V1.vx * V2.vz #B
        C = V1.vx * V2.vy - V1.vy * V2.vx #C
        D = -A * pt1.x - B * pt1.y - C * pt1.z #D
        arr = [A, B, C, D] #arr
        new(A, B, C, D, arr)
    end
end

"""
### Прямая на плоскости, определяемая 2-мя точками
поле `arr` : массив [A, B, C] коэффициентов уравнения прямой.
"""
struct line2d
    A::Float64
    B::Float64
    C::Float64
    L::Float64
    Directive::vector2d
    arr::Vector{Float64}

    function line2d(pt1::point3d, pt2::point3d)
        V = vector3d(pt1, pt2)
        A = V.vy
        B = -V.vx
        C = V.vx*pt1.x+V.vy*pt1.x
        L = sqrt(A^2 + B^2)
        Directive = vector2d(V.vx, V.vy)
        arr = [A, B, C]
        new(A, B, C, L, Directive, arr)
    end

    function line2d(pt1::point3d!, pt2::point3d!)
        V = vector3d(pt1, pt2)
        A = V.vy
        B = -V.vx
        C = V.vx*pt1.x+V.vy*pt1.x
        L = sqrt(A^2 + B^2)
        Directive = vector2d(V.vx, V.vy)
        arr = [A, B, C]
        new(A, B, C, L, Directive, arr)
    end

    function line2d(pt1::point2d, pt2::point2d)
        Directive = vector2d(pt1, pt2)
        A = Directive.vy
        B = -Directive.vx
        C = Directive.vx * pt1.x + Directive.vy * pt1.x
        L = sqrt(A^2 + B^2)
        arr = [A, B, C]
        new(A, B, C, L, Directive, arr)
    end

    function line2d(pt1::point2d!, pt2::point2d!)
        Directive = vector2d(pt1, pt2)
        A = Directive.vy
        B = -Directive.vx
        C = Directive.vx * pt1.x + Directive.vy * pt1.x
        L = sqrt(A^2 + B^2)
        arr = [A, B, C]
        new(A, B, C, L, Directive, arr)
    end
end

"""
### Интерполяция на основе уравнения прямой или плоскости
"""
function Interpolation(line::line2d, x::Real)
    if (line.B == 0) return Inf
    else return (-line.A * x - line.C) / line.B
    end
end

function Interpolation(plane::plane, x::Real, y::Real)
    if (plane.C == 0) return Inf
    else return (-plane.A * x - plane.B * y - plane.D) / plane.C
    end
end
