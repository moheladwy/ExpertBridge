import React, { useState } from "react";
import ReactPlayer from "react-player";
import {
	Carousel,
	CarouselContent,
	CarouselItem,
	CarouselNext,
	CarouselPrevious,
} from "@/views/components/ui/carousel";
import { MediaObjectResponse } from "@/features/media/types";
import {
	Dialog,
	DialogContent,
} from "@/views/components/ui/dialog";

interface PostMediaCarouselProps {
	medias: MediaObjectResponse[];
}

const MediaCarousel: React.FC<PostMediaCarouselProps> = ({ medias }) => {
	const [open, setOpen] = useState(false);
	const [picToBeOpened, setPicToBeOpened] = useState(0);
	const [activeMediaIndex, setActiveMediaIndex] = useState(0);

	if (!medias || medias.length === 0) return null;

	const handleOpen = (index: number) => {
		setPicToBeOpened(index);
		setOpen(true);
	};

	const handleClose = () => {
		setOpen(false);
	};

	return (
		<>
			<Dialog open={open} onOpenChange={(isOpen) => !isOpen && handleClose()}>
				<DialogContent className="max-w-5xl">
					{medias?.[picToBeOpened]?.url ? (
						<img
							src={medias[picToBeOpened].url}
							alt="Post content"
							className="max-w-full max-h-[90vh] object-contain"
						/>
					) : (
						<div className="p-4 text-center">
							<p>No media available</p>
						</div>
					)}
				</DialogContent>
			</Dialog>
			{/* Media */}
			<div className="aspect-auto flex justify-center items-center w-full rounded-md relative">
				<Carousel
					onSlideChange={(index: number) => {
						setActiveMediaIndex(index);
					}}
				>
					<CarouselContent>
						{medias.map((media, index) => (
							<CarouselItem
								key={index}
								className="cursor-pointer"
							>
								{media.type.startsWith("video") ? (
									<ReactPlayer
										url={media.url}
										width="100%"
										height="100%"
										controls
									/>
								) : (
									<img
										src={media.url}
										alt={`media-${index}`}
										onClick={() => handleOpen(index)}
										className="w-full h-full object-cover"
									/>
								)}
							</CarouselItem>
						))}
					</CarouselContent>

					{medias.length > 1 && (
						<>
							<div className="absolute top-1/2 left-14 -translate-y-1/2 z-20 max-sm:hidden">
								<CarouselPrevious />
							</div>
							<div className="absolute top-1/2 right-14 -translate-y-1/2 z-10 max-sm:hidden">
								<CarouselNext />
							</div>
						</>
					)}
				</Carousel>
			</div>

			{/* Media Dots */}
			{medias.length > 1 && (
				<div className="flex justify-center items-center mt-1 gap-2">
					{medias.map((_, index) => (
						<span
							key={index}
							className={`w-2 max-md:w-1.5 h-2 max-md:h-1.5 rounded-full ${index === activeMediaIndex ? "bg-main-blue" : "bg-gray-400"}`}
						/>
					))}
				</div>
			)}
		</>
	);
};

export default MediaCarousel;
