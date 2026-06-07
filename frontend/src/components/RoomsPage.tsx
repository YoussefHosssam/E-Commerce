import { useEffect, useRef } from "react";
import type { WheelEvent } from "react";
import { roomsData, type RoomGalleryItem } from "../data/roomsData";

type RoomsPageProps = {
  onExploreRoom: (slug: string) => void;
  initialSlug?: string;
};

export function RoomsPage({ onExploreRoom, initialSlug }: RoomsPageProps) {
  return <RoomSlider rooms={roomsData} onExploreRoom={onExploreRoom} initialSlug={initialSlug} />;
}

export function RoomSlider({
  rooms,
  onExploreRoom,
  initialSlug,
}: {
  rooms: RoomGalleryItem[];
  onExploreRoom: (slug: string) => void;
  initialSlug?: string;
}) {
  const slideRefs = useRef<Array<HTMLElement | null>>([]);

  useEffect(() => {
    if (!initialSlug) {
      return;
    }

    const index = rooms.findIndex((room) => room.slug === initialSlug);
    slideRefs.current[index]?.scrollIntoView({ block: "start" });
  }, [initialSlug, rooms]);

  const handleWheel = (event: WheelEvent<HTMLElement>) => {
    if (Math.abs(event.deltaX) <= Math.abs(event.deltaY)) {
      return;
    }

    const currentIndex = slideRefs.current.findIndex((slide) => {
      if (!slide) {
        return false;
      }

      const rect = slide.getBoundingClientRect();
      return rect.top <= window.innerHeight * 0.35 && rect.bottom >= window.innerHeight * 0.35;
    });

    const nextIndex = event.deltaX > 0 ? currentIndex + 1 : currentIndex - 1;
    const nextSlide = slideRefs.current[Math.min(Math.max(nextIndex, 0), rooms.length - 1)];

    if (nextSlide) {
      event.preventDefault();
      nextSlide.scrollIntoView({ behavior: "smooth", block: "start" });
    }
  };

  return (
    <main id="top" className="rooms-page" onWheel={handleWheel}>
      {rooms.map((room, index) => (
        <RoomSlide
          key={room.slug}
          room={room}
          onExploreRoom={onExploreRoom}
          refCallback={(node) => {
            slideRefs.current[index] = node;
          }}
        />
      ))}
    </main>
  );
}

export function RoomSlide({
  room,
  onExploreRoom,
  refCallback,
}: {
  room: RoomGalleryItem;
  onExploreRoom: (slug: string) => void;
  refCallback: (node: HTMLElement | null) => void;
}) {
  return (
    <section className="room-slide" ref={refCallback} aria-label={room.name}>
      <div className="room-media" aria-hidden="true">
        <img src={room.image} alt="" style={{ objectPosition: room.objectPosition ?? "center" }} />
        <img
          className="room-media-secondary"
          src={room.secondaryImage ?? room.image}
          alt=""
          style={{ objectPosition: room.secondaryObjectPosition ?? "70% center" }}
        />
      </div>
      <div className="room-slide-overlay" />
      <div className="room-slide-content">
        <p>{room.description}</p>
        <h1>{room.name}</h1>
        <RoomExploreButton onClick={() => onExploreRoom(room.slug)} />
      </div>
    </section>
  );
}

export function RoomExploreButton({ onClick }: { onClick: () => void }) {
  return (
    <button className="room-explore-button" type="button" onClick={onClick}>
      Explore
    </button>
  );
}
