"use client";

import React, { useState, useEffect } from "react";
import ReactPlayer from "react-player";
import {
	Carousel,
	CarouselContent,
	CarouselItem,
	CarouselNext,
	CarouselPrevious,
	type CarouselApi,
} from "@/components/ui/carousel";
import { MediaObjectResponse } from "@/features/media/types";
import { Dialog, DialogContent } from "@/components/ui/dialog";
import Image from "next/image";

interface PostMediaCarouselProps {
	medias: MediaObjectResponse[];
}

const MediaCarousel: React.FC<PostMediaCarouselProps> = ({ medias }) => {
	const [open, setOpen] = useState(false);
	const [picToBeOpened, setPicToBeOpened] = useState(0);
	const [activeMediaIndex, setActiveMediaIndex] = useState(0);
	const [carouselApi, setCarouselApi] = useState<CarouselApi>();

	useEffect(() => {
		if (!carouselApi) return;

		const onSelect = () => {
			setActiveMediaIndex(carouselApi.selectedScrollSnap());
		};

		carouselApi.on("select", onSelect);
		onSelect(); // Set initial index

		return () => {
			carouselApi.off("select", onSelect);
		};
	}, [carouselApi]);

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
			<Dialog
				open={open}
				onOpenChange={(isOpen) => !isOpen && handleClose()}
			>
				<DialogContent className="max-w-5xl">
					{medias?.[picToBeOpened]?.url ? (
						<Image
							src={medias[picToBeOpened].url}
							alt="Post content"
							width={1200}
							height={800}
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
				<Carousel setApi={setCarouselApi}>
					<CarouselContent>
						{medias.map((media, index) => (
							<CarouselItem
								key={index}
								className="cursor-pointer"
							>
								{media.type.startsWith("video") ? (
									<ReactPlayer
										src={media.url}
										width="100%"
										height="100%"
										controls
									/>
								) : (
									<Image
										src={media.url}
										alt={`media-${index}`}
										width={800}
										height={600}
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
							className={`w-2 max-md:w-1.5 h-2 max-md:h-1.5 rounded-full ${
								index === activeMediaIndex
									? "bg-main-blue"
									: "bg-gray-400"
							}`}
						/>
					))}
				</div>
			)}
		</>
	);
};

export default MediaCarousel;
