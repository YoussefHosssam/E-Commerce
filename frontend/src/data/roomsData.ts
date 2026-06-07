export type RoomGalleryItem = {
  slug: string;
  name: string;
  image: string;
  secondaryImage?: string;
  description: string;
  objectPosition?: string;
  secondaryObjectPosition?: string;
};

export const roomsData: RoomGalleryItem[] = [
  {
    slug: "oslo",
    name: "Oslo",
    image:
      "https://images.unsplash.com/photo-1505693416388-ac5ce068fe85?auto=format&fit=crop&w=1800&q=90",
    secondaryImage:
      "https://images.unsplash.com/photo-1616594039964-ae9021a400a0?auto=format&fit=crop&w=900&q=90",
    description: "Minimal calm living space shaped by pale timber, soft daylight, and quiet proportions.",
    objectPosition: "center",
    secondaryObjectPosition: "center",
  },
  {
    slug: "new-york",
    name: "New York",
    image:
      "https://images.unsplash.com/photo-1618221195710-dd6b41faaea6?auto=format&fit=crop&w=1800&q=90",
    secondaryImage:
      "https://images.unsplash.com/photo-1600210492486-724fe5c67fb0?auto=format&fit=crop&w=900&q=90",
    description: "Modern city apartment with sculptural seating, strong silhouettes, and gallery-like restraint.",
    objectPosition: "center",
    secondaryObjectPosition: "60% center",
  },
  {
    slug: "lisbon",
    name: "Lisbon",
    image:
      "https://images.unsplash.com/photo-1616486338812-3dadae4b4ace?auto=format&fit=crop&w=1800&q=90",
    secondaryImage:
      "https://images.unsplash.com/photo-1600607688969-a5bfcd646154?auto=format&fit=crop&w=900&q=90",
    description: "Warm colorful lounge with relaxed textures, sunlit walls, and an easy coastal rhythm.",
    objectPosition: "center",
    secondaryObjectPosition: "center",
  },
];
