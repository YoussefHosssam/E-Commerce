import type { CollageImage, MenuGroup, Product, Room, Store } from "../types/commerce";

export const heroImage =
  "https://images.unsplash.com/photo-1586023492125-27b2c045efd7?auto=format&fit=crop&w=2200&q=90";

export const collageBackgroundImage =
  "https://images.unsplash.com/photo-1600607688969-a5bfcd646154?auto=format&fit=crop&w=2200&q=85";

export const navLinks = [
  { label: "Lookbook", href: "#lookbook" },
  { label: "Shop", href: "/shop" },
];

export const products: Product[] = [
  {
    id: "modern-chair",
    slug: "modern-chair",
    name: "Modern Chair",
    price: "$ 249.00 USD",
    category: "chair",
    image:
      "https://images.unsplash.com/photo-1586023492125-27b2c045efd7?auto=format&fit=crop&w=900&q=85",
    description:
      "A sculptural lounge chair with a soft wool seat, warm tone shell, and a quiet modern silhouette.",
    badge: "NEW",
  },
  {
    id: "elegant-lamp",
    slug: "elegant-lamp",
    name: "Elegant Lamp",
    price: "$ 129.00 USD",
    category: "lamp",
    image:
      "https://images.unsplash.com/photo-1507473885765-e6ed057f782c?auto=format&fit=crop&w=900&q=85",
    description:
      "A refined pendant lamp with a burnished metallic finish and a calm diffused glow.",
  },
  {
    id: "black-chair",
    slug: "black-chair",
    name: "Black Chair",
    price: "$ 199.00 USD",
    category: "chair",
    image:
      "https://images.unsplash.com/photo-1598300042247-d088f8ab3a91?auto=format&fit=crop&w=900&q=85",
    description:
      "A low black accent chair designed for quiet rooms, deep textures, and minimal interiors.",
  },
];

export const rooms: Room[] = [
  {
    id: "oslo",
    label: "Oslo",
    image:
      "https://images.unsplash.com/photo-1505693416388-ac5ce068fe85?auto=format&fit=crop&w=900&q=85",
  },
  {
    id: "new-york",
    label: "New York",
    image:
      "https://images.unsplash.com/photo-1618221195710-dd6b41faaea6?auto=format&fit=crop&w=900&q=85",
  },
  {
    id: "lisbon",
    label: "Lisbon",
    image:
      "https://images.unsplash.com/photo-1616486338812-3dadae4b4ace?auto=format&fit=crop&w=900&q=85",
  },
];

export const shopProducts: Product[] = [
  products[0],
  products[1],
  products[2],
  {
    id: "lisbon-sofa",
    slug: "lisbon-sofa",
    name: "Lisbon Sofa",
    price: "$ 399.00 USD",
    category: "sofa",
    image:
      "https://images.unsplash.com/photo-1616486338812-3dadae4b4ace?auto=format&fit=crop&w=900&q=85",
    description:
      "A generous sofa with a low profile, soft linen texture, and quiet proportions for relaxed rooms.",
  },
  {
    id: "retro-chair",
    slug: "retro-chair",
    name: "Retro Chair",
    price: "$ 299.00 USD",
    category: "chair",
    image:
      "https://images.unsplash.com/photo-1611464908623-07f19927264d?auto=format&fit=crop&w=900&q=85",
    description:
      "A retro lounge chair with rounded volume, saturated upholstery, and a sculptural living-room presence.",
  },
  {
    id: "black-lounge-chair",
    slug: "black-lounge-chair",
    name: "Black Chair",
    price: "$ 199.00 USD",
    category: "chair",
    image:
      "https://images.unsplash.com/photo-1601392740426-907c7b028119?auto=format&fit=crop&w=900&q=85",
    description:
      "A deep black lounge chair with a grounded silhouette, built for calm interiors and reading corners.",
  },
];

export const stores: Store[] = [
  {
    id: "hamburg",
    city: "Hamburg",
    address: ["Solve Store,", "22765 Hamburg"],
    image:
      "https://images.unsplash.com/photo-1518005020951-eccb494ad742?auto=format&fit=crop&w=600&q=85",
  },
  {
    id: "lisbon-store",
    city: "Lisbon",
    address: ["Solve Store,", "1049 Lisbon"],
    image:
      "https://images.unsplash.com/photo-1600607688969-a5bfcd646154?auto=format&fit=crop&w=600&q=85",
  },
];

export const menuGroups: MenuGroup[] = [
  {
    title: "SOLVE",
    links: ["Home", "Rooms", "Journal", "Contact"],
  },
  {
    title: "SHOP",
    links: ["Home", "Rooms", "Lookbook", "Contact"],
  },
];

export const collageImages: CollageImage[] = [
  {
    id: "collage-chair-top",
    image: products[0].image,
    alt: "Yellow and gray modern chair",
    className:
      "left-[38%] top-[-35px] size-[86px] sm:size-[104px] lg:size-[120px]",
  },
  {
    id: "collage-lamp-left",
    image: products[1].image,
    alt: "Copper pendant lamps",
    className:
      "left-[9%] top-[112px] size-[80px] sm:left-[17%] sm:size-[96px] lg:size-[105px]",
  },
  {
    id: "collage-chair-lower",
    image: products[2].image,
    alt: "Black chair in dark room",
    className:
      "bottom-[58px] left-[5%] size-[82px] sm:left-[8%] sm:size-[102px] lg:size-[115px]",
  },
  {
    id: "collage-room-bottom",
    image: rooms[0].image,
    alt: "Minimal interior room",
    className:
      "bottom-[-18px] left-[28%] size-[72px] sm:left-[22%] sm:size-[88px] lg:size-[95px]",
  },
  {
    id: "collage-room-right",
    image: rooms[1].image,
    alt: "Interior with plants",
    className:
      "right-[12%] top-[118px] size-[88px] sm:right-[22%] sm:size-[110px] lg:size-[125px]",
  },
  {
    id: "collage-sofa-right",
    image: rooms[2].image,
    alt: "Warm sofa room detail",
    className:
      "right-[6%] top-[270px] hidden size-[94px] sm:block lg:size-[105px]",
  },
];
