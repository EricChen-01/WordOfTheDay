import { useState, useEffect, useRef } from "react"
import { Provider } from "./components/ui/provider"
import {
  Box,
  Container,
  Flex,
  Heading,
  Text,
  Badge,
  Separator,
  Spinner,
  VStack,
  HStack,
} from "@chakra-ui/react"
import "./App.css"

const API_URL = import.meta.env.VITE_API_URL || "https://wordoftheday.azurewebsites.net/api/WordOfTheDay"
console.log("API_URL:", API_URL)

function getLocalDate() {
  const now = new Date()
  const yyyy = now.getFullYear()
  const mm = String(now.getMonth() + 1).padStart(2, "0")
  const dd = String(now.getDate()).padStart(2, "0")
  return `${yyyy}-${mm}-${dd}`
}

// "jlpt-n2" -> "N2"
function formatLevel(level) {
  if (!level) return null
  const match = level.match(/n[1-5]/i)
  return match ? match[0].toUpperCase() : level
}

function App() {
  const [word, setWord] = useState(null)
  const [status, setStatus] = useState("loading") // loading | success | error
  const hasFetched = useRef(false)

  useEffect(() => {
    if (hasFetched.current) return
    hasFetched.current = true

    const localDate = getLocalDate()

    fetch(`${API_URL}`)
      .then((res) => {
        if (!res.ok) throw new Error(`API returned ${res.status}`)
        return res.json()
      })
      .then((data) => {
        setWord(data)
        setStatus("success")
      })
      .catch((err) => {
        console.error("Failed to fetch word of the day:", err)
        setStatus("error")
      })
  }, [])

  return (
    <Provider>
      <Box minH="100vh" bg="bg.canvas">
        <Flex
          as="header"
          justify="space-between"
          align="center"
          px={8}
          py={5}
          borderBottom="1px solid"
          borderColor="ai.100"
        >
          <Heading size="md" color="fg.default" fontFamily="heading">
            Word of the Day
          </Heading>
          <Text fontSize="sm" color="ai.400">
            {new Date().toLocaleDateString("en-US", {
              weekday: "long",
              month: "long",
              day: "numeric",
            })}
          </Text>
        </Flex>

        <Container maxW="lg" py={16}>
          {status === "loading" && (
            <VStack py={20} gap={4}>
              <Spinner size="lg" color="ai.500" borderWidth="3px" />
              <Text color="ai.400">Fetching today's word…</Text>
            </VStack>
          )}

          {status === "error" && (
            <Box
              bg="white"
              borderRadius="lg"
              border="1px solid"
              borderColor="ai.100"
              p={10}
              textAlign="center"
            >
              <Text color="shu.500" fontWeight="medium">
                Couldn't load today's word.
              </Text>
              <Text color="ai.400" fontSize="sm" mt={2}>
                Check that the API is running and try refreshing.
              </Text>
            </Box>
          )}

          {status === "success" && word && (
            <Box
              bg="white"
              borderRadius="lg"
              border="1px solid"
              borderColor="ai.100"
              p={10}
              textAlign="center"
              boxShadow="sm"
            >
              {word.level && (
                <Badge colorPalette="orange" mb={4}>
                  {formatLevel(word.level)}
                </Badge>
              )}

              <Heading fontFamily="heading" fontSize="5xl" color="fg.default" mb={2}>
                {word.word}
              </Heading>
              <Text color="ai.400" fontSize="lg" mb={6}>
                {word.furigana}
              </Text>

              <Separator mb={6} borderColor="ai.100" />

              <Text fontSize="xl" color="fg.default" mb={4}>
                {word.meaning}
              </Text>

              {word.partsOfSpeech && word.partsOfSpeech.length > 0 && (
                <HStack justify="center" gap={2} flexWrap="wrap" mt={4}>
                  {word.partsOfSpeech.map((pos, i) => (
                    <Badge key={i} variant="subtle" colorPalette="gray" fontSize="xs">
                      {pos}
                    </Badge>
                  ))}
                </HStack>
              )}
            </Box>
          )}
        </Container>
      </Box>
    </Provider>
  )
}

export default App