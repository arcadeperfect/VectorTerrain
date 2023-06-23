using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GeometeryUtils2D
{
    // from thread: https://forum.unity.com/threads/line-intersection.17384/
    public static bool LineIntersection( Vector2 p1,Vector2 p2, Vector2 p3, Vector2 p4, ref Vector2 intersection )
    {
        float Ax,Bx,Cx,Ay,By,Cy,d,e,f,num/*,offset*/;
        float x1lo,x1hi,y1lo,y1hi;
        
        Ax = p2.x-p1.x;
        Bx = p3.x-p4.x;
        
        // X bound box test/
        if(Ax<0) {
            x1lo=p2.x; x1hi=p1.x;
        } else {
            x1hi=p2.x; x1lo=p1.x;
        }
        
        if(Bx>0) {
            if(x1hi < p4.x || p3.x < x1lo) return false;
        } else {
            if(x1hi < p3.x || p4.x < x1lo) return false;
        }
        
        Ay = p2.y-p1.y;
        By = p3.y-p4.y;
        
        // Y bound box test//
     
        if(Ay<0) {
            y1lo=p2.y; y1hi=p1.y;
        } else {
            y1hi=p2.y; y1lo=p1.y;
        }
        
        if(By>0) {
            if(y1hi < p4.y || p3.y < y1lo) return false;
        } else {
            if(y1hi < p3.y || p4.y < y1lo) return false;
        }
        
        Cx = p1.x-p3.x;
        Cy = p1.y-p3.y;
        d = By*Cx - Bx*Cy;  // alpha numerator//
        f = Ay*Bx - Ax*By;  // both denominator//
        
        // alpha tests//
     
        if(f>0) {
            if(d<0 || d>f) return false;
        } else {
            if(d>0 || d<f) return false;
        }
        
        e = Ax*Cy - Ay*Cx;  // beta numerator//
        
        // beta tests //
     
        if(f>0) {
            if(e<0 || e>f) return false;
        } else {
            if(e>0 || e<f) return false;
        }
        
        // check if they are parallel
     
        if(f==0) return false;
           
        // compute intersection coordinates //
     
        num = d*Ax; // numerator //
     
    //    offset = same_sign(num,f) ? f*0.5f : -f*0.5f;   // round direction //
     
    //    intersection.x = p1.x + (num+offset) / f;
        intersection.x = p1.x + num / f;
        num = d*Ay;
     
    //    offset = same_sign(num,f) ? f*0.5f : -f*0.5f;
     
    //    intersection.y = p1.y + (num+offset) / f;
        intersection.y = p1.y + num / f;
        
        return true;
    }
    
    
    // doesn't return intersection point, just bool
    public static bool FasterLineIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4) {
   
        Vector2 a = p2 - p1;
        Vector2 b = p3 - p4;
        Vector2 c = p1 - p3;
   
        float alphaNumerator = b.y*c.x - b.x*c.y;
        float alphaDenominator = a.y*b.x - a.x*b.y;
        float betaNumerator  = a.x*c.y - a.y*c.x;
        float betaDenominator  = a.y*b.x - a.x*b.y;
   
        bool doIntersect = true;
   
        if (alphaDenominator == 0 || betaDenominator == 0) {
            doIntersect = false;
        } else {
       
            if (alphaDenominator > 0) {
                if (alphaNumerator < 0 || alphaNumerator > alphaDenominator) {
                    doIntersect = false;
               
                }
            } else if (alphaNumerator > 0 || alphaNumerator < alphaDenominator) {
                doIntersect = false;
            }
       
            if (doIntersect && betaDenominator > 0) {
                if (betaNumerator < 0 || betaNumerator > betaDenominator) {
                    doIntersect = false;
                }
            } else if (betaNumerator > 0 || betaNumerator < betaDenominator) {
                doIntersect = false;
            }
        }
 
        return doIntersect;
    }
}
