import { clsx, type ClassValue } from "clsx"
import { twMerge } from "tailwind-merge"
import { MembershipType } from "../types"

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs))
}

export function getMembershipTypeLabel(type: MembershipType): string {
  switch (type) {
    case MembershipType.Student:
      return "Student"
    case MembershipType.Adult:
      return "Adult"
    case MembershipType.Senior:
      return "Senior"
    case MembershipType.Staff:
      return "Staff"
    default:
      return "Unknown"
  }
}

export function getMembershipTypeBadgeVariant(type: MembershipType): "default" | "secondary" | "outline" | "destructive" {
  switch (type) {
    case MembershipType.Student:
      return "default"
    case MembershipType.Adult:
      return "secondary"
    case MembershipType.Senior:
      return "outline"
    case MembershipType.Staff:
      return "destructive"
    default:
      return "outline"
  }
}
